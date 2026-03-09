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
        private readonly Dictionary<string, CancellationTokenSource> _thumbnailCtsMap = new();
        private FolderItem? _currentThumbnailFolder;

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

        // ✅ 全部图片总数（所有已打开的根文件夹）
        public int TotalImageCountAll
        {
            get
            {
                int total = 0;
                foreach (var root in FolderTree)
                {
                    total += _fileScanner.GetAllFiles(root).Count;
                }
                return total;
            }
        }

        // ✅ 当前选中文件夹的图片数
        public int TotalImageCountCurrent =>
            SelectedFolder != null ? _fileScanner.GetAllFiles(SelectedFolder).Count : 0;

        // ✅ 缩略图加载进度（仅当前文件夹）
        public double LoadProgress =>
            Resources.Count > 0
                ? (Resources.Count(r => r.Thumbnail != null) * 100.0 / Resources.Count)
                : 0;

        // ✅ 是否正在加载缩略图（用于控制进度条显示）
        public bool IsLoadingThumbnails => Resources.Any(r => r.Thumbnail == null);

        [RelayCommand]
        private void ExitApplication()
        {
            foreach (var cts in _thumbnailCtsMap.Values)
            {
                cts.Cancel();
                cts.Dispose();
            }
            _thumbnailCtsMap.Clear();
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

            // 取消上一个文件夹的缩略图加载
            if (_currentThumbnailFolder != null && _currentThumbnailFolder != value)
            {
                if (_thumbnailCtsMap.TryGetValue(_currentThumbnailFolder.FullPath, out var oldCts))
                {
                    oldCts.Cancel();
                    oldCts.Dispose();
                    _thumbnailCtsMap.Remove(_currentThumbnailFolder.FullPath);
                }
            }

            _currentThumbnailFolder = value;

            // 同步加载文件列表
            var allFiles = _fileScanner.GetAllFiles(value);
            Resources = new ObservableCollection<ResourceItem>(allFiles);

            // 启动缩略图加载
            var cts = new CancellationTokenSource();
            _thumbnailCtsMap[value.FullPath] = cts;

            foreach (var file in Resources)
            {
                if (cts.Token.IsCancellationRequested) break;
                _ = file.LoadThumbnailAsync(cts.Token);
            }

            // 通知状态栏相关属性变化
            OnPropertyChanged(nameof(TotalImageCountAll));
            OnPropertyChanged(nameof(TotalImageCountCurrent));
            OnPropertyChanged(nameof(LoadProgress));
            OnPropertyChanged(nameof(IsLoadingThumbnails));
        }
    }
}
