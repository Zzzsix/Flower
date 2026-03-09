using System.Windows.Media.Imaging;

namespace AssetManager.Services
{
    public interface IThumbnailService
    {
        BitmapSource? GetThumbnail(string filePath);
    }
}
