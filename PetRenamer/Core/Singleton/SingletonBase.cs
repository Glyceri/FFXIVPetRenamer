namespace PetRenamer.Core.Singleton;

public abstract class SingletonBase<T> where T : SingletonBase<T>
{
    protected static T _instance = null!;
    public static T Instance
    {
        get => _instance;
    }
}
