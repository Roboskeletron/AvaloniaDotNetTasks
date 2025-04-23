namespace SteelPlant.Domain;

public class LoaderFactory
{
    ILoader CreateLoader(int materialQuantity, int loadingTimeMs) => new Loader(materialQuantity, loadingTimeMs);
}
