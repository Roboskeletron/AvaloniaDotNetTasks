using ReactiveUI;
using SteelPlant.Domain;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SteelPlant.UI.ViewModels;

public class WorkerViewModel : ReactiveObject
{
    private readonly Worker _worker;
    private readonly ChannelReader<FurnaceEventArgs> _channelReader;

    private bool _isWorking;
    public bool IsWorking
    {
        get => _isWorking;
        set => this.RaiseAndSetIfChanged(ref _isWorking, value);
    }

    public WorkerViewModel(int coolingTimeMs, ChannelReader<FurnaceEventArgs> channelReader)
    {
        _worker = new Worker(coolingTimeMs);
        _channelReader = channelReader;
    }

    public async Task StartWorkingAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var eventItem = await _channelReader.ReadAsync(cancellationToken);

            if (eventItem == null)
            {
                continue;
            }

            IsWorking = true;

            await _worker.CoolFurnaceAsync(eventItem.Furnace, cancellationToken);

            IsWorking = false;
        }
    }
}
