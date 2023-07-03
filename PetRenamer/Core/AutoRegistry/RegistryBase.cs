using System;
using System.Collections.Generic;
using System.Reflection;
using PetRenamer.Core.AutoRegistry.Interfaces;
using System.Linq;

namespace PetRenamer.Core.AutoRegistry;

internal class RegistryBase<T, TT> : IdentifyableRegistryBase where T : IRegistryElement where TT : Attribute
{
    protected List<T> elements = new List<T>();

    public RegistryBase()
    {
        Type elementType = typeof(T);
        Assembly elementAssembly = Assembly.GetAssembly(elementType)!;
        Type[] elementTypes = elementAssembly.GetTypes().Where(t =>
            t.IsClass &&
            !t.IsAbstract &&
            t.IsSubclassOf(typeof(T)) &&
            t.GetCustomAttribute<TT>() != null)
        .ToArray();

        OnTypeArrayCreation(elementTypes);

        foreach (Type type in elementTypes)
        {
            T createdElement = (T)Activator.CreateInstance(type)!;
            OnElementCreation(createdElement);
            elements.Add(createdElement);
        }
    }

    public T GetElement(Type elementType)
    {
        foreach (T element in elements)
            if (element.GetType() == elementType)
                return element;

        return default!;
    }

    internal void ClearAllElements()
    {
        foreach (T element in elements)
        {
            OnElementDestroyed(element);
            if(element.GetType().IsSubclassOf(typeof(IDisposable)))
                ((IDisposable)element)?.Dispose();
        }
        elements.Clear();
    }

    protected virtual void OnTypeArrayCreation(Type[] types) { }
    protected virtual void OnElementCreation(T element) { }
    protected virtual void OnElementDestroyed(T element) { }
}
