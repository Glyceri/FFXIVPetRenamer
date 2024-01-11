using ImGuiNET;
using System.Numerics;

namespace PetRenamer.Windows.Bonus.SnowAnim;

internal class Snow : AnimatableElement
{
    float _visibility;
    float _speed;
    float _flickerMin;
    float _flickerMax;
    float _flickerSpeed = SNOW_POWER;
    const float SNOW_POWER = 0.4f;

    public Snow(Vector2 position, float size, float speed, Vector3 flickerData) : base(position, size)
    {
        _speed = speed * 0.1f;
        _visibility = flickerData.Z;
        _flickerMin = flickerData.X;
        _flickerMax = flickerData.Y;
    }

    internal override void Update(double deltaTime)
    {
        Flicker(deltaTime);
        Translate(new Vector2(0, (float)(_speed * deltaTime)));
    }

    void Flicker(double power)
    {
        _visibility += (float)(power * _flickerSpeed);
        if (_visibility < 0) _visibility = 0;
        if (_visibility > 1) _visibility = 1;
        if (_flickerSpeed > 0 && _visibility >= _flickerMax) _flickerSpeed = -SNOW_POWER;
        if (_flickerSpeed < 0 && _visibility <= _flickerMin) _flickerSpeed = SNOW_POWER;
    }

    internal override uint GetColour() => GetColour(1, 1, 1, _visibility);
    internal override void Draw(ImDrawListPtr ptr, Vector2 screenPosition) => ptr.AddCircleFilled(screenPosition, Size, GetColour());
}
