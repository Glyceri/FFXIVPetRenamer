using ImGuiNET;
using System.Numerics;

namespace PetRenamer.Windows.Bonus.RainAnim;

internal class Rain : AnimatableElement
{
    float internalSize = 0f;
    float _minusPoint = 0f;
    float _rainMax = 0f;
    float _shrinkSpeed = 0f;

    public Rain(Vector2 position, float size, float minusPoint, float rainMax, float shrinkSpeed) : base(position, size)
    {
        _minusPoint = -minusPoint;
        internalSize = _minusPoint;
        _rainMax = rainMax;
        _shrinkSpeed = shrinkSpeed;
    }

    internal override void Draw(ImDrawListPtr ptr, Vector2 screenPosition)
    {
        if (internalSize <= 0) return;
        ptr.AddCircle(screenPosition, Size + internalSize, GetColour());
    }

    internal override void Update(double deltaTime)
    {
        internalSize += (float)deltaTime * _shrinkSpeed;
        if (internalSize >= _rainMax) internalSize = _minusPoint;
    }
}
