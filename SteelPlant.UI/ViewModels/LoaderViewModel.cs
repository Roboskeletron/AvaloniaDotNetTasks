using ReactiveUI;
using SteelPlant.Domain;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SteelPlant.UI.ViewModels;

public class LoaderViewModel : ReactiveObject
{
    private readonly ILoader _loader;
    private readonly ChannelReader<FurnaceEventArgs> _channelReader;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public LoaderViewModel(int materialQuantity, int loadingTimeMs, ChannelReader<FurnaceEventArgs> channelReader)
    {
        _loader = LoaderFactory.CreateLoader(materialQuantity, loadingTimeMs);
        _channelReader = channelReader;
    }

    public async Task StartLoadingAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var eventItem = await _channelReader.ReadAsync(cancellationToken);

            if (eventItem == null)
            {
                continue;
            }

            IsLoading = true;

            await _loader.LoadMaterialAsync(eventItem.Furnace, cancellationToken);

            IsLoading = false;
        }
    }
}
