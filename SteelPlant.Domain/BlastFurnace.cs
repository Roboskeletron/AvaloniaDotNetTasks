namespace SteelPlant.Domain;

public enum FurnaceStatus
{
    Normal,
    Overheated
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
    }

    public async Task OperateCycleAsync(CancellationToken cancellationToken)
    {
        if (Status == FurnaceStatus.Overheated)
        {
            return;
        }

        await Task.Delay(_operationDelayMs, cancellationToken);
        _currentMaterial = Math.Max(_currentMaterial - (_maxCapacity / 20), 0);

        if (_currentMaterial == 0)
        {
            MaterialDepleted?.Invoke(this, new FurnaceEventArgs(this));
        }

        if (_random.NextDouble() < _overheatProbability)
        {
            Status = FurnaceStatus.Overheated;
            Overheated?.Invoke(this, new FurnaceEventArgs(this));
        }
    }

    public void CoolDown()
    {
        Status = FurnaceStatus.Normal;
    }
}
