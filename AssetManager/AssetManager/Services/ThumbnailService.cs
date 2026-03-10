using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AssetManager.Services;

public static class ThumbnailService
{
    /// <summary>
    /// 异步生成指定宽度的缩略图，在解码阶段完成缩放，避免模糊和高内存占用。
    /// </summary>
    /// <param name="filePath">图像文件路径</param>
    /// <param name="targetWidth">目标宽度（高度自动按比例）</param>
    /// <returns>缩略图 BitmapSource，失败返回 null</returns>
    public static async Task<BitmapSource?> GetThumbnail(string filePath, int targetWidth = 400)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return null;

        // 在后台线程创建 BitmapImage（安全，因使用 OnLoad + Freeze）
        return await Task.Run(() =>
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                // 关键设置：解码时直接缩放到目标宽度
                bitmap.DecodePixelWidth = targetWidth;
                bitmap.UriSource = new Uri(filePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;      // 立即加载并释放文件句柄
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();
                bitmap.Freeze(); // 使对象跨线程可用（可绑定到 UI）
                return bitmap;
            }
            catch
            {
                // 可选：记录日志
                return null;
            }
        });
    }
}