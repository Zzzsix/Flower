using System.IO;

namespace Flower.Services
{
    public class FileScanService
    {
        private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".ico", ".webp"
        };

        public async Task<List<string>> GetImageFilesRecursiveAsync(string path)
        {
            return await Task.Run(() =>
            {
                var files = new List<string>();
                try
                {
                    foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
                    {
                        if (ImageExtensions.Contains(Path.GetExtension(file)))
                            files.Add(file);
                    }
                }
                catch { }
                return files;
            });
        }
    }
}