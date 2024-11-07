using ArchiBase.Data;
using ArchiBase.Models;
using IronSoftware.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ArchiBase.Utils;

public class ThumbnailService(IWebHostEnvironment webHostEnvironment, ModelContext modelContext)
{
    public IWebHostEnvironment WebHostEnvironment { get; } = webHostEnvironment;
    public ModelContext ModelContext { get; } = modelContext;

    public string GetThumbnail(Photo photo)
    {
        if (photo.ThumbnailData == null)
        {
            var wwwroot = WebHostEnvironment.WebRootPath;
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

                ModelContext.SaveChanges();
            }
        }
        return Convert.ToBase64String(photo.ThumbnailData ?? []);
    }
}