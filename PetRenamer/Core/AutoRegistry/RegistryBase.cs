using PetRenamer.Core.Attributes;
using PetRenamer.Core.AutoRegistry.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        Sort();
        OnAllRegistered();
    }

    void Sort()
    {
        if (!typeof(SortableAttribute).IsAssignableFrom(typeof(TT))) return;

        List<(int, int)> orderAndValueList = new List<(int, int)> ();

        for(int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] is not SortableAttribute sAttribute) continue;
            orderAndValueList.Add((sAttribute.Order, i));
        }

        orderAndValueList.Sort((val1, val2) => val1.Item1.CompareTo(val2.Item1));

        List<T> newListT = new List<T>();
        List<TT> newListTT = new List<TT>();

        for (int i = 0; i < orderAndValueList.Count; i++)
        {
            (int, int) vals = orderAndValueList[i];
            newListT.Add(elements[vals.Item2]);
            newListTT.Add(attributes[vals.Item2]);
        }

        elements = newListT;
        attributes = newListTT;
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
