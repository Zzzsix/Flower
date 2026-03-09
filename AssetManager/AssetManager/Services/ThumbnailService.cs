using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AssetManager.Services
{
    public class ThumbnailService : IThumbnailService
    {
        public static BitmapSource? GetThumbnail(string filePath)
        {
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnLoad);
                var frame = decoder.Frames[0];

                const int thumbSize = 200;
                var scale = Math.Min(thumbSize / (double)frame.PixelWidth, thumbSize / (double)frame.PixelHeight);
                var width = (int)(frame.PixelWidth * scale);
                var height = (int)(frame.PixelHeight * scale);

                var transformed = new TransformedBitmap(frame, new ScaleTransform(scale, scale));
                transformed.Freeze(); // ✅ 关键：冻结后可跨线程、节省内存

                return transformed;
            }
            catch
            {
                return null;
            }
        }

        BitmapSource? IThumbnailService.GetThumbnail(string filePath) => GetThumbnail(filePath);
    }
}
