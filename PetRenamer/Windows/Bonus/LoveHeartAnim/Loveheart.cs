using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PetRenamer.Windows.Bonus.LoveHeartAnim;

internal class Loveheart : AnimatableElement
{
    Vector2[] points = new Vector2[resolution];

    float _speed;
    uint redColour;

    public Loveheart(Vector2 position, float size, float speed) : base(position, size)
    {
        _speed = speed * 0.1f;
        redColour = GetColour(new Vector4(1, 0.3f, 0.3f, 1));
        FillPoints();
    }

    void FillPoints()
    {
        float t = 0f;
        float step = 2f * MathF.PI / (float)resolution;

        for (int i = 0; i < resolution; i++)
        {
            float x = (16f * MathF.Pow(MathF.Sin(t), 3));
            float y = -(13f * MathF.Cos(t) - 5f * MathF.Cos(2f * t) - 2f * MathF.Cos(3f * t) - MathF.Cos(4f * t));
            y -= Size;
            points[i] = new Vector2(x, y);
            t += step;
        }
    }

    internal override void Update(double deltaTime)
    {
        Translate(new Vector2(0, (float)(-_speed * deltaTime)));
    }

    const int resolution = 20; // Number of points to draw

    internal override void Draw(ImDrawListPtr ptr, Vector2 screenPosition)
    {
        float calcedSize = Size * (1 - MathF.Abs(0.7f - Position.Y)) * 0.1f;

        List<Vector2> newPoints = new List<Vector2>();

        for(int i = 0; i < resolution; i++)
        {
            newPoints.Add(points[i] * calcedSize + screenPosition);
        }

        ptr.AddConvexPolyFilled(ref newPoints.ToArray()[0], newPoints.Count, redColour);
    }
}
