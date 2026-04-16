using MediatR;

namespace AleonAPI.Features.Sites.Commands.DeleteSite;

public class DeleteSiteHandler(ApplicationDbContext context)
    : IRequestHandler<DeleteSiteCommand, bool>
{
    public async Task<bool> Handle(DeleteSiteCommand request, CancellationToken ct)
    {
        var site = await context.Sites.FindAsync(request.SiteId, ct);
        if (site == null) return false;

        context.Sites.Remove(site);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
