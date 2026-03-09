using AssetManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;

namespace AssetManager.Models;

public partial class ResourceItem : ObservableObject
{
    private static readonly SemaphoreSlim _semaphore = new(4, 4); // 全局最多 4 个并发

    [ObservableProperty]
    private string fileName = string.Empty;

    [ObservableProperty]
    private string filePath = string.Empty;

    [ObservableProperty]
    private BitmapSource? thumbnail;

    public async Task LoadThumbnailAsync(CancellationToken cancellationToken)
    {
        // 1. 在排队前就检查是否已取消（避免无谓等待）
        if (cancellationToken.IsCancellationRequested)
            return;

        try
        {
            // 2. 等待信号量，支持取消
            await _semaphore.WaitAsync(cancellationToken);

            // 3. 再次检查（可能在 Wait 期间被取消）
            if (cancellationToken.IsCancellationRequested)
                return;

            // 4. 在后台线程执行耗时操作（但无法中途取消）
            var bitmap = await Task.Run(() =>
            {
                // 注意：这里仍无法响应取消，但至少我们限制了并发
                if (cancellationToken.IsCancellationRequested)
                    return null!;
                return ThumbnailService.GetThumbnail(FilePath);
            }, cancellationToken);

            // 5. 最终检查：只有未取消才更新 UI
            if (bitmap != null && !cancellationToken.IsCancellationRequested)
            {
                Thumbnail = bitmap;
                OnPropertyChanged(nameof(Thumbnail)); // 确保通知
            }
        }
        catch (OperationCanceledException)
        {
            // 正常取消，不处理
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load thumbnail for {FilePath}: {ex}");
        }
        finally
        {
            if (_semaphore.CurrentCount < 4) // 防止 Release 多次
                _semaphore.Release();
        }
    }
}