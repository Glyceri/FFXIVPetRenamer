using ImGuiNET;
using PetRenamer.Core.AutoRegistry.Interfaces;
using System.Numerics;

namespace PetRenamer.Windows.Bonus;

internal abstract class AnimatableElement : IDisposableRegistryElement
{
    internal AnimatableElement(Vector2 position, float size)
    {
        _position = position;
        _size = size;
    }

    internal abstract void Draw(ImDrawListPtr ptr, Vector2 screenPosition);
    internal virtual void Update(double deltaTime) { }
    internal virtual void OnDipose() { }
    public void Dispose() => OnDipose();
    internal virtual uint GetColour() => GetColour(1, 1, 1, 1);

    float _size;
    public float Size => _size;

    Vector2 _position = Vector2.Zero;
    public Vector2 Position => _position;
    internal void SetPosition(Vector2 position) => _position = position;
    internal void Translate(Vector2 translation)
    {
        float calcatedSize = 0.25f; // shouldn't be 0.5 but the size calculated over the objects scale depending on the area they are in and UGH
        _position += translation;
        if (_position.X > 1 + calcatedSize) _position.X -= 1 + calcatedSize * 2;
        if (_position.X < 0 - calcatedSize) _position.X += 1 + calcatedSize * 2;
        if (_position.Y > 1 + calcatedSize) _position.Y -= 1 + calcatedSize * 2;
        if (_position.Y < 0 - calcatedSize) _position.Y += 1 + calcatedSize * 2;
    }

    protected uint GetColour(float r, float g, float b, float a) => GetColour(new Vector4(r, g, b, a));
    protected uint GetColour(Vector4 color)
    {
        uint ret = (byte)(color.W * 255);
        ret <<= 8;
        ret += (byte)(color.Z * 255);
        ret <<= 8;
        ret += (byte)(color.Y * 255);
        ret <<= 8;
        ret += (byte)(color.X * 255);
        return ret;
    }
}
