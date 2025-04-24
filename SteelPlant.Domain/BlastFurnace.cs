namespace SteelPlant.Domain;

public enum FurnaceStatus
{
    Normal,
    Overheated,
    Empty,
}

public class FurnaceEventArgs : EventArgs
{
    public BlastFurnace Furnace { get; }

    public FurnaceEventArgs(BlastFurnace furnace)
    {
        Furnace = furnace;
    }
}

public class BlastFurnace
{
    private readonly int _maxCapacity;
    private readonly double _overheatProbability;
    private readonly int _operationDelayMs;
    private int _currentMaterial = 0;
    private readonly Random _random = new();

    public FurnaceStatus Status { get; private set; }

    public event EventHandler<FurnaceEventArgs> MaterialDepleted;
    public event EventHandler<FurnaceEventArgs> Overheated;

    public BlastFurnace(int maxCapacity, double overheatProbability, int operationDelayMs)
    {
        _maxCapacity = maxCapacity;
        _overheatProbability = overheatProbability;
        _operationDelayMs = operationDelayMs;
        Status = FurnaceStatus.Normal;
    }

    public void AddMaterial(int quantity)
    {
        _currentMaterial = Math.Min(_currentMaterial + quantity, _maxCapacity);
        Status = FurnaceStatus.Normal;
    }

    public async Task OperateCycleAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(_operationDelayMs, cancellationToken);

        if (Status == FurnaceStatus.Overheated || Status == FurnaceStatus.Empty)
        {
            return;
        }

        _currentMaterial = Math.Max(_currentMaterial - (_maxCapacity / 20), 0);

        if (_currentMaterial == 0)
        {
            Status = FurnaceStatus.Empty;
            MaterialDepleted?.Invoke(this, new FurnaceEventArgs(this));
            return;
        }

        if (_random.NextDouble() < _overheatProbability)
        {
            Status = FurnaceStatus.Overheated;
            Overheated?.Invoke(this, new FurnaceEventArgs(this));
            return;
        }
    }

    public void CoolDown()
    {
        Status = FurnaceStatus.Normal;
    }
}
