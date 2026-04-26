using CommunityToolkit.Mvvm.ComponentModel;
using Flower.Models;
using Flower.Services;
using System.Collections.ObjectModel;


namespace Flower.ViewModels
{
    public partial class FolderViewModel : ObservableObject
    {
        private readonly FolderScan _scan;

        public FolderTree Tree { get; }

        public string Name => Tree.Name;
        public string FullPath => Tree.FullPath;

        public ObservableCollection<FolderViewModel> Children { get; }
            = new();

        [ObservableProperty]
        private bool isExpanded;

        [ObservableProperty]
        private bool isLoaded;

        //控制是否显示三角形
        public bool HasChildren { get; }

        public FolderViewModel(FolderTree tree, FolderScan scan)
        {
            Tree = tree;
            _scan = scan;

            HasChildren = _scan.HasSubFolders(tree.FullPath);
            if (HasChildren)
            {
                // 懒加载占位符
                Children.Add(null);
            }
        }

        partial void OnIsExpandedChanged(bool value)
        {
            if (value && !IsLoaded)
            {
                LoadChildrenAsync();
            }
        }

        private async void LoadChildrenAsync()
        {
            IsLoaded = true;
            Children.Clear();

            var folders = await _scan.GetSubFoldersAsync(FullPath);

            foreach (var folder in folders)
            {
                Children.Add(new FolderViewModel(folder, _scan));
            }
        }
    }
}
