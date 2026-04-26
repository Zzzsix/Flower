using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Flower.Models
{
    public class FolderTree
    {
        public string Name { get; set; }
        public string FullPath { get; set; }

        public FolderTree(string name, string fullPath)
        {
            Name = name;
            FullPath = fullPath;
        }
    }
}
