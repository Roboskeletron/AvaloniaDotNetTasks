using ReactiveUI;
using SteelPlant.Domain;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SteelPlant.UI.ViewModels;

public class FurnaceParams { public int MaxCapacity { get; set; } public double OverheatProbability { get; set; } public int OperationDelayMs { get; set; } }

public class LoaderParams { public int Quantity { get; set; } public int IntervalMs { get; set; } }

public class WorkerParams { public int CoolingTimeMs { get; set; } }

public class MainWindowViewModel : ViewModelBase
{
    private readonly Channel<FurnaceEventArgs> _depletedChannel = Channel.CreateUnbounded<FurnaceEventArgs>();
    private readonly Channel<FurnaceEventArgs> _overheatChannel = Channel.CreateUnbounded<FurnaceEventArgs>();
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public ObservableCollection<FurnaceViewModel> FurnaceItems { get; } = new();
    public ObservableCollection<LoaderViewModel> LoaderItems { get; } = new();
    public ObservableCollection<WorkerViewModel> WorkerItems { get; } = new();

    public FurnaceParams NewFurnace { get; } = new();
    public LoaderParams NewLoader { get; } = new();
    public WorkerParams NewWorker { get; } = new();

    public ReactiveCommand<Unit, Unit> AddFurnaceCommand { get; }
    public ReactiveCommand<Unit, Unit> AddLoaderCommand { get; }
    public ReactiveCommand<Unit, Unit> AddWorkerCommand { get; }

    public MainWindowViewModel()
    {
        AddFurnaceCommand = ReactiveCommand.Create(() =>
        {
            var furnace = new FurnaceViewModel(NewFurnace.MaxCapacity, NewFurnace.OverheatProbability, NewFurnace.OperationDelayMs, _overheatChannel.Writer, _depletedChannel.Writer);
            FurnaceItems.Add(furnace);
            Task.Run(async () => await furnace.StartOperatingAsync(_cancellationTokenSource.Token));
        });

        AddLoaderCommand = ReactiveCommand.Create(() =>
        {
            var loader = new LoaderViewModel(NewLoader.Quantity, NewLoader.IntervalMs, _depletedChannel.Reader);
            LoaderItems.Add(loader);
            Task.Run(async () => await loader.StartLoadingAsync(_cancellationTokenSource.Token));
        });

        AddWorkerCommand = ReactiveCommand.Create(() =>
        {
            var worker = new WorkerViewModel(NewWorker.CoolingTimeMs, _overheatChannel.Reader);
            WorkerItems.Add(worker);
            Task.Run(async () => await worker.StartWorkingAsync(_cancellationTokenSource.Token));
        });
    }

    public void StopAll()
    {
        _cancellationTokenSource.Cancel();
    }
}
