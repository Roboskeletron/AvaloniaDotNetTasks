using ReactiveUI;
using SteelPlant.Domain;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SteelPlant.UI.ViewModels;

public class FurnaceViewModel : ReactiveObject
{
    private readonly BlastFurnace _blastFurnace;
    private readonly ChannelWriter<FurnaceEventArgs> _overheateChannelWriter;
    private readonly ChannelWriter<FurnaceEventArgs> _itemsDepletedChannelWriter;

    private bool _isOperating;
    public bool IsOperating
    {
        get => _isOperating;
        set => this.RaiseAndSetIfChanged(ref _isOperating, value);
    }

    private FurnaceStatus _status;
    public FurnaceStatus Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    public FurnaceViewModel(
        int maxCapacity,
        double overheatProbability,
        int operationDelayMs,
        ChannelWriter<FurnaceEventArgs> overheateChannelWriter,
        ChannelWriter<FurnaceEventArgs> itemsDepletedChannelWriter)
    {
        _blastFurnace = new BlastFurnace(maxCapacity, overheatProbability, operationDelayMs);
        _overheateChannelWriter = overheateChannelWriter;
        _itemsDepletedChannelWriter = itemsDepletedChannelWriter;
    }

    public async Task StartOperatingAsync(CancellationToken cancellationToken)
    {
        _blastFurnace.MaterialDepleted += (_, e) =>
        {
            _itemsDepletedChannelWriter.TryWrite(e);
        };

        _blastFurnace.Overheated += (_, e) =>
        {
            _overheateChannelWriter.TryWrite(e);
        };

        while (!cancellationToken.IsCancellationRequested)
        {
            await _blastFurnace.OperateCycleAsync(cancellationToken);

            Status = _blastFurnace.Status;
            IsOperating = Status == FurnaceStatus.Normal;
        }
    }
}
