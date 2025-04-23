namespace SteelPlant.Domain;

internal class Loader : ILoader
{
    private readonly int _quantity;
    private readonly int _loadingTimeMs;

    public Loader(int quantity, int loadingTimeMs)
    {
        _quantity = quantity;
        _loadingTimeMs = loadingTimeMs;
    }

    public async Task LoadMaterialAsync(BlastFurnace blastFurnace, CancellationToken cancellationToken)
    {
        await Task.Delay(_loadingTimeMs, cancellationToken);
        blastFurnace.AddMaterial(_quantity);
    }
}
