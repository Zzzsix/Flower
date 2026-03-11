using AssetManager.Models;
using System.Collections.ObjectModel;

namespace AssetManager.Services;

public class ThumbnailLoaderService
{
    private readonly SemaphoreSlim _semaphore = new(8);

    public async Task LoadAsync(
        ObservableCollection<ResourceItem> resources,
        CancellationToken token)
    {
        var tasks = resources.Select(r => LoadSingleAsync(r, token));
        await Task.WhenAll(tasks);
    }

    private async Task LoadSingleAsync(ResourceItem item, CancellationToken token)
    {
        try
        {
            await _semaphore.WaitAsync(token);

            var bitmap = await ThumbnailService.GetThumbnail(item.FilePath);

            if (!token.IsCancellationRequested && bitmap != null)
            {
                item.Thumbnail = bitmap;
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _semaphore.Release();
        }
    }
}