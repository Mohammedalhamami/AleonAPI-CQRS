using AleonAPI.Services;

namespace AleonAPI.Features.Countries.Commands.ImportCountries;

public class ImportCountriesHandler(ImportTaskQueue taskQueue) : IRequestHandler<ImportCountriesCommand>
{
    public async Task Handle(ImportCountriesCommand request, CancellationToken ct)
    {
        // Boom! 0 millisecond execution. It just throws the string Path into the queue and returns immediately!
        await taskQueue.QueueFileAsync(request.FilePath, ct);
    }
}
