using Flower.Models;
using System.IO;

namespace Flower.Services
{
    public class FolderScan
    {
        public FolderTree LoadFolder(string path)
        {
            var root = new FolderTree
            {
                Name = Path.GetFileName(path),
                FullPath = path
            };

            LoadChildren(root);

            return root;
        }

        private void LoadChildren(FolderTree parent)
        {
            try
            {
                var directories = Directory.GetDirectories(parent.FullPath);

                foreach (var dir in directories)
                {
                    var child = new FolderTree
                    {
                        Name = Path.GetFileName(dir),
                        FullPath = dir
                    };

                    parent.Children.Add(child);

                    // 递归加载
                    LoadChildren(child);
                }
            }
            catch
            {
                // 权限问题直接忽略（行业标准做法）
            }
        }
    }
}