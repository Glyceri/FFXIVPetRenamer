using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.Windows.Bonus.FireworkAnim;

internal class Firework : AnimatableElement
{
    bool shootMode = true;
    float explodeTimer = 0;

    float _explosionSize = 0;
    float _height = 0;


    const int lineCount = 10;
    List<Vector2> lines = new List<Vector2>();

    public Firework(Vector2 position, float size, float height, float explosionSize) : base(position, size)
    {
        _height = height;
        _explosionSize = explosionSize;
        float stepsize = 360 / (float)lineCount;
        for (int i = 0; i < lineCount; i++)
            lines.Add(RotatedVector(new Vector2(0, 1), stepsize * i));
    }

    private const double DegToRad = Math.PI / 180;
    Vector2 RotatedVector(Vector2 line, float degrees)
    {
        double rad = degrees * DegToRad;

        double ca = Math.Cos(rad);
        double sa = Math.Sin(rad);

        return new Vector2((float)(ca * line.X - sa * line.Y), (float)(sa * line.X + ca * line.Y));
    }

    internal override void Update(double deltaTime)
    {
        if (shootMode)
        {
            Translate(new Vector2(0, -(float)deltaTime));

            if (Position.Y < _height) 
            {
                explodeTimer = 0;
                shootMode = false; 
            }
        }
        else
        {
            explodeTimer += (float)deltaTime;
            if (explodeTimer >= 1) Reset();
        }
    }

    internal override void Draw(ImDrawListPtr ptr, Vector2 screenPosition)
    {
        if(shootMode) ptr.AddLine(screenPosition, screenPosition + new Vector2(0, 10), GetColour());
        else
        {
            for(int i = 0; i < lines.Count; i++)
                ptr.AddLine(screenPosition, screenPosition + (lines[i] * explodeTimer * _explosionSize), GetColour());
        }
    }

    void Reset()
    {
        shootMode = true;
        explodeTimer = 0;
        SetPosition(new Vector2(Position.X, 1));
    }
}
