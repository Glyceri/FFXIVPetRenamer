using ImGuiNET;
using System.Numerics;

namespace PetRenamer.Windows.Bonus.RainAnim;

internal class FallingRain : AnimatableElement
{
    float _speed;

    public FallingRain(Vector2 position, float size, float speed) : base(position, size) 
    {
        _speed = speed;
    }

    internal override void Draw(ImDrawListPtr ptr, Vector2 screenPosition)
    {
        ptr.AddLine(screenPosition, screenPosition + new Vector2(0, Size * 2), GetColour());
    }

    internal override void Update(double deltaTime)
    {
        Translate(new Vector2(0, _speed * (float)deltaTime));
    }
}
