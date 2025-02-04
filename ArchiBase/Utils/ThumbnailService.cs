using ArchiBase.Data;
using ArchiBase.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ArchiBase.Utils;

public class ThumbnailService(IWebHostEnvironment webHostEnvironment, ModelContext modelContext)
{
    private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;
    private readonly ModelContext modelContext = modelContext;

    public string GetThumbnail(Photo photo)
    {
        GenerateSingleThumbnail(photo);
        modelContext.SaveChanges();
        return Convert.ToBase64String(photo.ThumbnailData ?? []);
    }

    public void GenerateThumbnail(Photo photo)
    {
        GenerateSingleThumbnail(photo);
        modelContext.SaveChanges();
    }

    public void GenerateThumbnails(IEnumerable<Guid> photoIds)
    {
        var photos = modelContext.Photos.Where(p => photoIds.Contains(p.Id));
        foreach (var photo in photos) GenerateSingleThumbnail(photo);
        modelContext.SaveChanges();
    }

    private void GenerateSingleThumbnail(Photo photo)
    {
        if (photo.ThumbnailData == null)
        {
            var wwwroot = webHostEnvironment.WebRootPath;
            if (!photo.IsHidden && File.Exists(wwwroot + photo.PhotoLink))
            {
                using var image = Image.Load(wwwroot + photo.PhotoLink);
                var scaleFactor = Math.Max(image.Width / 240.0, image.Height / 180.0);
                if (scaleFactor <= 0) scaleFactor = 1;
                image.Mutate(
                    x => x.Resize(
                        (int)Math.Round(image.Width / scaleFactor), (int)Math.Round(image.Height / scaleFactor),
                        KnownResamplers.Lanczos3
                        )
                );
                using var ms = new MemoryStream();
                image.Save(ms, new WebpEncoder { });
                photo.ThumbnailData = ms.ToArray();
            }
        }
    }
}