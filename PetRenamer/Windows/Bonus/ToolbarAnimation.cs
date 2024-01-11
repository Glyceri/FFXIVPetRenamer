using ImGuiNET;
using PetRenamer.Core.Attributes;
using PetRenamer.Core.AutoRegistry.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.Windows.Bonus;

internal abstract class ToolbarAnimation : IDisposableRegistryElement, IInitializable, IClearable
{
    internal virtual void Update(double deltaTime)
    {
        int count = _animatableElements.Count;
        for (int i = 0; i < count; i++)
            _animatableElements[i].Update(deltaTime);
    }
    internal void Draw(ImDrawListPtr drawListPtr, Vector2 startingPoint, Vector2 endPoint)
    {
        int count = _animatableElements.Count;
        for (int i = 0; i < count; i++)
            OnDrawElement(_animatableElements[i], drawListPtr, GetDrawPos(_animatableElements[i].Position, startingPoint, endPoint));
    }
    internal virtual void OnDrawElement(AnimatableElement element, ImDrawListPtr drawListPtr, Vector2 screenPosition) => element.Draw(drawListPtr, screenPosition);
    internal abstract void OnInitialize();
    internal virtual void OnClear() { }
    internal virtual void OnDispose() { }
    public void Initialize() => OnInitialize();

    public void Clear()
    {
        foreach (AnimatableElement animatableElement in _animatableElements)
            animatableElement?.Dispose();
        _animatableElements.Clear();
        OnClear();
    }

    public void Dispose()
    {
        Clear();
        OnDispose();
    }

    Random random = new Random();
    protected Random Random => random;
    protected float GetRandom(float range = 1) => GetRandomRange(0, range);
    protected float GetRandomRange(float min = 0, float max = 1) => ((float)Random.NextDouble() * (max - min)) + min;
    protected Vector2 GetRandomPos() => new Vector2(GetRandomRange(-0.25f, 1.25f), GetRandomRange(-0.25f, 1.25f));

    List<AnimatableElement> _animatableElements = new List<AnimatableElement>();
    protected List<AnimatableElement> animatableElements => _animatableElements;
    protected List<T> GetListAs<T>() where T : AnimatableElement
    {
        List<T> elementList = new List<T>();
        foreach(AnimatableElement animatableElement in _animatableElements)
            if (animatableElement is T tElement)
                elementList.Add(tElement);
        return elementList;
    }
    protected void AddElement<T>(T element) where T : AnimatableElement => _animatableElements?.Add(element);
    protected Vector2 GetDrawPos(Vector2 elementPos, Vector2 startingPoint, Vector2 endPoint) => new Vector2(Map(elementPos.X, 0, 1, startingPoint.X, endPoint.X), Map(elementPos.Y, 0, 1, startingPoint.Y, endPoint.Y));
    protected float Map(float value, float istart, float istop, float ostart, float ostop) => ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
}
