using ImGuiNET;
using System;
using System.Numerics;

namespace PetRenamer.Windows.Bonus.RotatingTriangleAnim;

internal class Triangle : AnimatableElement
{
    float _speed;
    float _angle;
    float _rotateSpeed;

    public Triangle(Vector2 position, float size, float speed, float angle, float rotateSpeed) : base(position, size)
    {
        _speed = speed * 0.1f;
        _angle = angle;
        _rotateSpeed = rotateSpeed;
    }

    internal override void Update(double deltaTime)
    {
        Translate(new Vector2(0, (float)(_speed * deltaTime)));
        _angle += _rotateSpeed * (float)deltaTime;
    }

    internal override uint GetColour() => GetColour(1, 1, 1, 1);
    internal override void Draw(ImDrawListPtr ptr, Vector2 screenPosition)
    {       
        ptr.AddTriangle(screenPosition + RotatedVector(new Vector2(0, Size * 2), _angle), screenPosition + RotatedVector(new Vector2(-Size, 0), _angle), screenPosition + RotatedVector(new Vector2(Size, 0), _angle), GetColour());
    }

    private const double DegToRad = Math.PI / 180;
    Vector2 RotatedVector(Vector2 line, float degrees) 
    {
        double rad = degrees * DegToRad;

        double ca = Math.Cos(rad);
        double sa = Math.Sin(rad);

        return new Vector2((float)(ca * line.X - sa * line.Y), (float)(sa * line.X + ca * line.Y));
    }
}
