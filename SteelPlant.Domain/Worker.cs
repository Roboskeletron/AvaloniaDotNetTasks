namespace SteelPlant.Domain;

public class Worker
{
    private readonly int _coolingTimeMs;

    public Worker(int coolingTimeMs)
    {
        _coolingTimeMs = coolingTimeMs;
    }

    public async Task CoolFurnaceAsync(BlastFurnace furnace, CancellationToken cancellationToken)
    {
        await Task.Delay(_coolingTimeMs, cancellationToken);
        furnace.CoolDown();
    }
}
