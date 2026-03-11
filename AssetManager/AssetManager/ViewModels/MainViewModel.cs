using AssetManager.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using AssetManager.Services;

namespace AssetManager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IFileScannerService _fileScanner;
        private readonly ThumbnailLoaderService _thumbnailLoader = new();
        private CancellationTokenSource? _thumbnailCts;

        public MainViewModel() : this(new FileScannerService()) { }

        public MainViewModel(IFileScannerService fileScanner)
        {
            _fileScanner = fileScanner;
        }

        [ObservableProperty]
        private ObservableCollection<FolderItem> folderTree = new();

        [ObservableProperty]
        private ObservableCollection<ResourceItem> resources = new();

        [ObservableProperty]
        private FolderItem? selectedFolder;

        [RelayCommand]
        private void ExitApplication()
        {
            _thumbnailCts?.Cancel();
            _thumbnailCts?.Dispose();

            Application.Current.Shutdown();
        }

        [RelayCommand]
        private async Task OpenFolder()
        {
            var dialog = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection."
            };

            if (dialog.ShowDialog() == true)
            {
                var folderPath = Path.GetDirectoryName(dialog.FileName)!;

                //避免重复添加相同根目录
                if (FolderTree.Any(f => f.FullPath.Equals(folderPath, StringComparison.OrdinalIgnoreCase)))
                {
                    SelectedFolder = FolderTree.First(f => f.FullPath.Equals(folderPath, StringComparison.OrdinalIgnoreCase));
                    return;
                }

                var root = _fileScanner.BuildFolderTree(folderPath);
                FolderTree.Add(root);
                SelectedFolder = root;
            }
        }

        partial void OnSelectedFolderChanged(FolderItem? value)
        {
            if (value == null) return;

            _thumbnailCts?.Cancel();
            _thumbnailCts?.Dispose();

            var allFiles = _fileScanner.GetAllFiles(value);
            Resources = new ObservableCollection<ResourceItem>(allFiles);

            _thumbnailCts = new CancellationTokenSource();
            _ = _thumbnailLoader.LoadAsync(Resources, _thumbnailCts.Token);
        }
    }
}
