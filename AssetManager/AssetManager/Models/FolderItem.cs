using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace AssetManager.Models
{
    public partial class FolderItem : ObservableObject
    {
        public string Name { get; set; } = string.Empty;

        public string FullPath { get; set; } = string.Empty;

        public ObservableCollection<FolderItem> Children { get; set; } = new();

        public ObservableCollection<ResourceItem> Files { get; set; } = new();
    }
}
