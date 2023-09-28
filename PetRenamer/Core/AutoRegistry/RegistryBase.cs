using System;
using System.Collections.Generic;
using System.Reflection;
using PetRenamer.Core.AutoRegistry.Interfaces;
using System.Linq;

namespace PetRenamer.Core.AutoRegistry;

internal class RegistryBase<T, TT> : IdentifyableRegistryBase where T : IRegistryElement where TT : Attribute
{
    protected List<T> elements = new List<T>();
    protected List<TT> attributes = new List<TT>();

    public RegistryBase()
    {
        OnSelfInitialize();
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
            T createdElement = CreateInstance(type)!;
            LocalOnElementCreation(createdElement);
            OnElementCreation(createdElement);
            elements.Add(createdElement);
            attributes.Add(createdElement.GetType().GetCustomAttribute<TT>()!);
        }

        foreach (T element in elements)
            OnLateElementCreation(element);

        OnAllRegistered();
    }

    public void Dispose()
    {
        OnDipose();
        ClearAllElements();
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
            if(element is IDisposable disposable) disposable.Dispose();
        }
        elements.Clear();
        attributes.Clear();
    }

    void LocalOnElementCreation(T element)
    {
        Type t = element.GetType();
        PropertyInfo[] properties = t.GetProperties();
        foreach (PropertyInfo property in properties)
            if (property.PropertyType == t && property.Name == "instance")
                property.SetValue(element, element);
    }

    protected virtual void OnSelfInitialize() { }
    protected virtual void OnDipose() { }
    protected virtual void OnAllRegistered() { }
    protected virtual void OnTypeArrayCreation(Type[] types) { }
    protected virtual void OnElementCreation(T element) { }
    protected virtual void OnLateElementCreation(T element) { }
    protected virtual void OnElementDestroyed(T element) { }

    protected virtual T CreateInstance(Type type) => (T)Activator.CreateInstance(type)!;
}
