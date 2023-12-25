using System.Numerics;

namespace PetRenamer.Windows.Bonus;

public class Snow
{
    Vector2 _position;
    float _size;
    float _visibility;
    float _speed;
    public Vector2 Position => _position;
    public float Size => _size;
    public float Visibility => _visibility;
    public float Speed => _speed;   

    float _flickerMin;
    float _flickerMax;

    float _flickerSpeed = SNOW_POWER;

    const float SNOW_POWER = 0.4f;

    public Snow(Vector2 position, float size, float speed, Vector3 flickerData)
    {
        SetPosition(position);
        _size = size;
        _speed = speed;
        _visibility = flickerData.Z;
        _flickerMin = flickerData.X;
        _flickerMax = flickerData.Y;
    }
    public void SetPosition(Vector2 position) => _position = position;
    public void Flicker(double power)
    {
        _visibility += (float)(power * _flickerSpeed);
        if (_visibility < 0) _visibility = 0;
        if (_visibility > 1) _visibility = 1;
        if (_flickerSpeed > 0 && _visibility >= _flickerMax) _flickerSpeed = -SNOW_POWER;
        if (_flickerSpeed < 0 && _visibility <= _flickerMin) _flickerSpeed = SNOW_POWER;
    }
}
