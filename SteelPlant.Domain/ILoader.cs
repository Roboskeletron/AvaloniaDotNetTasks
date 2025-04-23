namespace SteelPlant.Domain;

public interface ILoader
{
    Task LoadMaterialAsync(BlastFurnace blastFurnace, CancellationToken cancellationToken);
}
