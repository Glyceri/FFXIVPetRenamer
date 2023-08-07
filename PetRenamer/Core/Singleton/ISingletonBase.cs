namespace PetRenamer.Core.Singleton;

internal interface ISingletonBase<T> 
{
    public abstract static T instance { get; set; }
}
