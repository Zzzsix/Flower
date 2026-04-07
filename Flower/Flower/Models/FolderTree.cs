using System.Collections.ObjectModel;

namespace Flower.Models
{
    public class FolderTree
    {
        public string Name { get; set; }
        public string FullPath { get; set; }

        public ObservableCollection<FolderTree> Children { get; set; }
            = new ObservableCollection<FolderTree>();
    }
}
