using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flower.Models;
using Flower.Services;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace Flower.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly FolderScan _folderScan = new();
        private readonly FileScanService _fileScan = new();

        public ObservableCollection<FolderViewModel> Roots { get; } = new();
        public ObservableCollection<FileItemViewModel> Images { get; } = new();

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private FolderViewModel? selectedFolder;

        //打开文件夹命令
        [RelayCommand]
        private void LoadFolder()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var selectedPath = dialog.FileName;

                //检查重复添加
                if (Roots.Any(r => r.FullPath == selectedPath))
                {
                    ErrorMessage = "该文件夹已加载";
                    return;
                }

                var tree = new FolderTree(
                    Path.GetFileName(selectedPath),
                    selectedPath
                    );

                Roots.Add(new FolderViewModel(tree,_folderScan));
            }
        }

        //退出应用命令
        [RelayCommand]
        private void ExitApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        // 当选中文件夹变化时，递归加载其下所有图片
        partial void OnSelectedFolderChanged(FolderViewModel? value)
        {
            LoadImagesAsync(value);
        }

        private async void LoadImagesAsync(FolderViewModel? folder)
        {
            Images.Clear();
            if (folder == null) return;
            var files = await _fileScan.GetImageFilesRecursiveAsync(folder.FullPath);
            foreach (var file in files)
                Images.Add(new FileItemViewModel(file));
        }
    }
}