using ArchiBase.Data;
using ArchiBase.Models;
using CronScheduler.Extensions.Scheduler;

namespace ArchiBase.Utils;

public class PhotoApprovalJob(IServiceProvider provider) : IScheduledJob
{
    private readonly IServiceProvider provider = provider;

    public string Name => nameof(PhotoApprovalJob);

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var modelContext = scope.ServiceProvider.GetRequiredService<ModelContext>();
        var pendingPhotos = modelContext.Photos.Where(p => p.Status == PhotoStatus.Pending).ToList();
        var rejectedPhotos = modelContext.Photos.Where(p => p.Status == PhotoStatus.Rejected).ToList();
        var now = DateTime.UtcNow;
        foreach (var photo in pendingPhotos)
        {
            var hasPositiveVotes = photo.Votes.Upvotes > 0;
            var (approvalLimit, rejectionLimit) = (now - photo.PublicationDate).TotalDays switch
            {
                double d when d < 7 => (+2, -3),
                double d when d >= 7 && d < 14 => (+2, hasPositiveVotes ? -2 : -3),
                double d when d >= 14 && d < 28 => (+1, hasPositiveVotes ? -1 : -2),
                double d when d >= 28 && d < 56 => (0, hasPositiveVotes ? -1 : -2),
                _ => (0, -1)
            };
            if (photo.Votes.Votes >= approvalLimit) photo.Status = PhotoStatus.Published;
            if (photo.Votes.Votes <= rejectionLimit) photo.Status = PhotoStatus.Rejected;
        }
        foreach (var photo in rejectedPhotos)
        {
            if (photo.Votes.Votes >= +3) photo.Status = PhotoStatus.Published;
        }
        modelContext.UpdateRange(pendingPhotos);
        modelContext.UpdateRange(rejectedPhotos);
        await modelContext.SaveChangesAsync(cancellationToken);
    }
}

public class PhotoCleanupJob(IServiceProvider provider, IWebHostEnvironment webHostEnvironment) : IScheduledJob
{
    private readonly IServiceProvider provider = provider;
    private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;

    public string Name => nameof(PhotoCleanupJob);

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var modelContext = scope.ServiceProvider.GetRequiredService<ModelContext>();
        var now = DateTime.UtcNow;
        var photosToRemove = modelContext.Photos
            .Where(p => p.Status == PhotoStatus.Rejected && (now - p.PublicationDate).TotalDays > 30).ToList();
        // clean up the storage
        var wwwroot = webHostEnvironment.WebRootPath;
        foreach (var photo in photosToRemove)
        {
            File.Delete(wwwroot + photo.PhotoLink);
        }
        // clean up the database
        modelContext.RemoveRange(photosToRemove);
        await modelContext.SaveChangesAsync(cancellationToken);
    }
}