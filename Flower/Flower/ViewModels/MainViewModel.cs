using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flower.Models;
using Flower.Services;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Flower.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly FolderScan _folderService = new();

        [ObservableProperty]
        private ObservableCollection<FolderTree> folders = new();

        // 按钮命令
        [RelayCommand]
        private void LoadFolder()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Folders.Clear();

                var root = _folderService.LoadFolder(dialog.FileName);
                Folders.Add(root);
            }
        }
    }
}