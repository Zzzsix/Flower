using AssetManager.Models;

namespace AssetManager.Services
{
    public interface IFileScannerService
    {
        FolderItem BuildFolderTree(string rootPath);
        List<ResourceItem> GetAllFiles(FolderItem folder);
    }
}
