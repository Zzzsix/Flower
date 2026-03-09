using AssetManager.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace AssetManager.Services
{
    public class FileScannerService : IFileScannerService
    {
        private static readonly string[] SupportedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        // 递归构建文件夹树
        public FolderItem BuildFolderTree(string path)
        {
            var folder = new FolderItem
            {
                Name = Path.GetFileName(path),
                FullPath = path
            };

            try
            {
                var files = Directory.GetFiles(path)
                    .Where(f => SupportedImageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .Select(f => new ResourceItem
                    {
                        FileName = Path.GetFileName(f),
                        FilePath = f
                    })
                    .ToList();

                foreach (var file in files)
                    folder.Files.Add(file);

                var dirs = Directory.GetDirectories(path);
                foreach (var dir in dirs)
                {
                    try
                    {
                        folder.Children.Add(BuildFolderTree(dir));
                    }
                    catch (UnauthorizedAccessException) { /* skip */ }
                }
            }
            catch (UnauthorizedAccessException) { }

            return folder;
        }

        // 递归获取文件夹下所有子文件
        public List<ResourceItem> GetAllFiles(FolderItem folder)
        {
            var files = new List<string>();
            CollectFiles(folder, files);

            return files.Select(f => new ResourceItem
            {
                FilePath = f,
                FileName = Path.GetFileName(f)  // ✅ 修正：使用 FileName
            }).ToList();
        }

        // ✅ 新增：递归收集所有文件路径
        private void CollectFiles(FolderItem folder, List<string> files)
        {
            foreach (var file in folder.Files)
            {
                files.Add(file.FilePath);
            }

            foreach (var child in folder.Children)
            {
                CollectFiles(child, files);
            }
        }
    }
}
