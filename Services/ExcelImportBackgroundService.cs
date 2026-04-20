using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AleonAPI.Data;
using AleonAPI.Models;
using AleonAPI.Enums;
using ClosedXML.Excel;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AleonAPI.Services;

// Must inherit from BackgroundService so .NET spins it up continuously
public class ExcelImportBackgroundService(ImportTaskQueue taskQueue, IServiceScopeFactory scopeFactory, ILogger<ExcelImportBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Excel Import Background Worker is active and waiting for queues...");

        // This loop automatically pauses until taskQueue.ReadAllAsync() yields a new item!
        await foreach (var filePath in taskQueue.ReadAllAsync(stoppingToken))
        {
            try
            {
                logger.LogInformation($"Starting bulk import for file: {filePath}");
                await ProcessFileAsync(filePath, stoppingToken);
                logger.LogInformation($"SUCCESS: Finished bulk import for file: {filePath}");
                
                // Cleanup: Delete the big excel file from the hard drive now that we threw it in the DB!
                if (File.Exists(filePath)) File.Delete(filePath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"CRITICAL ERROR: Failed processing background import for file {filePath}");
            }
        }
    }

    private async Task ProcessFileAsync(string filePath, CancellationToken stoppingToken)
    {
        // RULE 1 OF BACKGROUND JOBS: You cannot directly inject DbContext into the constructor.
        // DbContext is strictly Scoped per HTTP Request. BackgroundServices are Singleton!
        // We must artificially open a temporary "Scope" to use the Database cleanly.
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var countriesToInsert = new List<Country>();

        // Open the physical file using FileStream
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var workbook = new XLWorkbook(fs);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            // Abort safely if the Server is shutting down via Ctrl+C
            if (stoppingToken.IsCancellationRequested) break;

            var country = new Country
            {
                Name = row.Cell(1).GetString(),
                Code = row.Cell(2).GetString(),
                Contenients = Enum.Parse<Continent>(row.Cell(3).GetString())
            };

            countriesToInsert.Add(country);

            // MEMORY SURVIVAL CHECK: If it is 100,000 rows, DO NOT save all at once! 
            // Memory will run out. Save into the Database in isolated "Chunks" of 5,000!
            if (countriesToInsert.Count >= 5000)
            {
                await context.Countries.AddRangeAsync(countriesToInsert, stoppingToken);
                await context.SaveChangesAsync(stoppingToken); // Commit chunk
                countriesToInsert.Clear(); // Empty the RAM!
            }
        }

        // Save any leftover remaining row stragglers that didn't divide evenly by 5,000
        if (countriesToInsert.Any())
        {
            await context.Countries.AddRangeAsync(countriesToInsert, stoppingToken);
            await context.SaveChangesAsync(stoppingToken);
        }
    }
}
