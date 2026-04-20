using AleonAPI.Endpoints.Artifact;
using AleonAPI.Endpoints.CatalogRecrod;
using AleonAPI.Endpoints.Country;
using AleonAPI.Endpoints.CustomIdentityEndpoints;
using AleonAPI.Endpoints.Home;
using AleonAPI.Endpoints.Sites;
using AleonAPI.Models;
using AleonAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = DataUtility.GetConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCustomSwagger();

// Register MediatR - scans all handlers in the assembly automatically
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Identity endpoints
builder.Services.AddIdentityApiEndpoints<User>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

// Email Sender Service
builder.Services.AddTransient<IEmailSender, ConsoleEmailService>();

builder.Services.AddValidation();
builder.Services.AddControllers();
// Register our insane background job!
builder.Services.AddSingleton<ImportTaskQueue>();
builder.Services.AddHostedService<ExcelImportBackgroundService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    await AleonAPI.Data.DataSeed.ManageDataAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapHomeEndpoints();
app.MapCustomIdentityEndpoints();
app.UseMiddleware<BlockIdentityEndpoints>();

app.MapSiteEndpoints();
app.MapArtifactMediaFileEndpoints();
app.MapArtifactEndpoints();
app.MapCatalogRecordEndpoints();
app.MapCountryEndpoints();
var authRouteGroup = app.MapGroup("/api/auth")
    .WithTags("Admin");
authRouteGroup.MapIdentityApi<User>();

app.Run();
