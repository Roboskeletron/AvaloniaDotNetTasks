namespace SteelPlant.Domain;

public class LoaderFactory
{
    public static ILoader CreateLoader(int materialQuantity, int loadingTimeMs) => new Loader(materialQuantity, loadingTimeMs);
}
