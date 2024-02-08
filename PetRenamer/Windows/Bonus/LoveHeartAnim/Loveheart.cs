using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.Windows.Bonus.LoveHeartAnim;

internal class Loveheart : AnimatableElement
{
    static Vector2[] points = new Vector2[LoveHeartSettings.LOVE_HEART_RESOLUTION];
    static bool pointsRegistered = false;

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
        if (pointsRegistered) return;
        float t = 0f;
        float step = 2f * MathF.PI / LoveHeartSettings.LOVE_HEART_RESOLUTION;

        for (int i = 0; i < LoveHeartSettings.LOVE_HEART_RESOLUTION; i++)
        {
            float x = (16f * MathF.Pow(MathF.Sin(t), 3));
            float y = -(13f * MathF.Cos(t) - 5f * MathF.Cos(2f * t) - 2f * MathF.Cos(3f * t) - MathF.Cos(4f * t));
            y -= Size;
            points[i] = new Vector2(x, y);
            t += step;
        }

        pointsRegistered = true;
    }

    internal override void Update(double deltaTime)
    {
        Translate(new Vector2(0, (float)(-_speed * deltaTime)));
    }

    internal override void Draw(ImDrawListPtr ptr, Vector2 screenPosition)
    {
        float calcedSize = Size * (1 - MathF.Abs(0.7f - Position.Y)) * 0.1f;

        List<Vector2> newPoints = new List<Vector2>();

        for(int i = 0; i < LoveHeartSettings.LOVE_HEART_RESOLUTION; i++)
        {
            newPoints.Add(points[i] * calcedSize + screenPosition);
        }

        ptr.AddConvexPolyFilled(ref newPoints.ToArray()[0], newPoints.Count, redColour);
    }
}
