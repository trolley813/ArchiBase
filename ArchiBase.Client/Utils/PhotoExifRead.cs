using ArchiBase.Models;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace ArchiBase.Components.Utils;

public static class PhotoExifReader
{
    public static (ExifData, ImpreciseDate, double, double) ReadExif(byte[] bytes)
    {
        using MemoryStream ms = new(bytes);
        ExifData exifData = new();
        ImpreciseDate shootingDate = new();
        double latitude = 0, longitude = 0;
        var directories = ImageMetadataReader.ReadMetadata(ms);
        // foreach (var d in directories)
        // {
        //     Console.WriteLine(d);
        //     foreach (var t in d.Tags)
        //     {
        //         Console.WriteLine(t);
        //     }
        // }
        var exifIFD0 = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
        var exifSub = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
        var exifGPS = directories.OfType<GpsDirectory>().FirstOrDefault();
        try
        {
            DateTime photoDate = DateTime.MinValue;
            bool dateRead = false;
            if (!dateRead) dateRead = exifSub?.TryGetDateTime(ExifSubIfdDirectory.TagDateTimeOriginal, out photoDate) ?? false;
            if (!dateRead) dateRead = exifSub?.TryGetDateTime(ExifSubIfdDirectory.TagDateTimeDigitized, out photoDate) ?? false;
            if (!dateRead) dateRead = exifIFD0?.TryGetDateTime(ExifIfd0Directory.TagDateTime, out photoDate) ?? false;
            if (!dateRead) throw new Exception("No date present");
            shootingDate = new ImpreciseDate { Date = photoDate, Precision = DatePrecision.Full };
            exifData.PhotoDate = photoDate;
        }
        catch (Exception ex)
        {
            shootingDate = new ImpreciseDate { Date = DateTime.MinValue, Precision = DatePrecision.None };
        }
        try
        {
            var latitudeTag = exifGPS?.GetRationalArray(GpsDirectory.TagLatitude);
            var latitudeRefTag = exifGPS?.GetString(GpsDirectory.TagLatitudeRef);
            if (latitudeTag is not null)
            {
                latitude = latitudeTag[0].ToDouble() + latitudeTag[1].ToDouble() / 60.0 + latitudeTag[2].ToDouble() / 3600.0;
                if (latitudeRefTag == "S") latitude = -latitude;
            }
            var longitudeTag = exifGPS?.GetRationalArray(GpsDirectory.TagLongitude);
            var longitudeRefTag = exifGPS?.GetString(GpsDirectory.TagLongitudeRef);
            if (longitudeTag is not null)
            {
                longitude = longitudeTag[0].ToDouble() + longitudeTag[1].ToDouble() / 60.0 + longitudeTag[2].ToDouble() / 3600.0;
                if (longitudeRefTag == "W") longitude = -longitude;
            }
        }
        catch (MetadataException ex)
        {
            throw;
        }
        exifData.Cameraman = exifIFD0?.GetString(ExifIfd0Directory.TagMake);
        exifData.CameraModel = exifIFD0?.GetString(ExifIfd0Directory.TagModel);
        exifData.Editor = exifIFD0?.GetString(ExifIfd0Directory.TagSoftware);
        exifData.AName = exifIFD0?.GetString(ExifIfd0Directory.TagArtist);
        exifData.Copy = exifSub?.GetString(ExifSubIfdDirectory.TagCopyright);
        exifData.TV = exifSub?.GetString(ExifSubIfdDirectory.TagExposureTime);
        exifData.AV = exifSub?.GetString(ExifSubIfdDirectory.TagAperture);
        if (exifSub?.TryGetInt32(ExifSubIfdDirectory.TagIsoEquivalent, out int resultISO) ?? false) exifData.ISO = resultISO;
        exifData.EV = exifSub?.GetString(ExifSubIfdDirectory.TagExposureBias);
        exifData.FL = exifSub?.GetString(ExifSubIfdDirectory.TagFocalLength);
        exifData.EFL = exifSub?.GetString(ExifSubIfdDirectory.Tag35MMFilmEquivFocalLength);
        if (exifSub?.TryGetInt32(ExifSubIfdDirectory.TagFlash, out int resultFlash) ?? false) exifData.Flash = resultFlash;
        if (exifSub?.TryGetInt32(ExifSubIfdDirectory.TagWhiteBalance, out int resultWB) ?? false) exifData.WB = resultWB;
        if (exifSub?.TryGetInt32(ExifSubIfdDirectory.TagMeteringMode, out int resultEM) ?? false) exifData.EMeter = resultEM;
        if (exifSub?.TryGetInt32(ExifSubIfdDirectory.TagExposureProgram, out int resultDZ) ?? false) exifData.SMode = resultDZ;
        exifData.DZoom = exifSub?.GetString(ExifSubIfdDirectory.TagDigitalZoomRatio);

        return (exifData, shootingDate, latitude, longitude);
    }
}