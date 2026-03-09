using AssetManager.Models;
using AssetManager.ViewModels;
using System.Windows;

namespace AssetManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //DataContext = new MainViewModel();
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm && e.NewValue is Models.FolderItem folder)
            {
                vm.SelectedFolder = folder; // 触发加载
            }
        }
    }
}