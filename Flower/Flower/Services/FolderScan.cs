using Flower.Models;
using System.IO;

namespace Flower.Services
{
    public class FolderScan
    {
        public async Task<List<FolderTree>> GetSubFoldersAsync(string path)
        {
            return await Task.Run(() =>
            {
                var result = new List<FolderTree>();

                try
                {
                    foreach (var dir in Directory.EnumerateDirectories(path))
                    {
                        result.Add(new FolderTree(
                            Path.GetFileName(dir),
                            dir
                        ));
                    }
                }
                catch { }

                return result;
            });
        }

        //判断是否有子文件夹
        public bool HasSubFolders(string path)
        {
            try
            {
                return Directory.EnumerateDirectories(path).Any();
            }
            catch
            {
                return false;
            }
        }
    }
}