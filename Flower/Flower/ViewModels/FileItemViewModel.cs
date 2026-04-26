using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Flower.ViewModels
{
    public partial class FileItemViewModel : ObservableObject
    {
        public string FullPath { get; }
        public string Name { get; }

        [ObservableProperty]
        private ImageSource? thumbnail;

        public FileItemViewModel(string fullPath)
        {
            FullPath = fullPath;
            Name = Path.GetFileName(fullPath);
            _ = LoadThumbnailAsync();
        }

        private async Task LoadThumbnailAsync()
        {
            try
            {
                var bmp = await Task.Run(() =>
                {
                    using var stream = new FileStream(FullPath, FileMode.Open, FileAccess.Read);
                    var frame = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                    // 固定缩略图宽度 200，高度按比例
                    if (frame.PixelWidth > 200)
                    {
                        var scale = 200.0 / frame.PixelWidth;
                        var transformed = new TransformedBitmap(frame, new ScaleTransform(scale, scale));
                        transformed.Freeze();
                        return (ImageSource)transformed;
                    }
                    return frame;
                });
                Thumbnail = bmp;
            }
            catch { }
        }
    }
}