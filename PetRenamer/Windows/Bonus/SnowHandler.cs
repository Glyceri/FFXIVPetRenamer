using Dalamud.Plugin.Services;
using ImGuiNET;
using PetRenamer.Core.Attributes;
using PetRenamer.Core.Handlers;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.Windows.Bonus;

public class SnowHandler : IDisposable, IInitializable
{
    const int SNOW_COUNT = 250;
    const int MAP_SIZE_X = 10;
    const int MAP_SIZE_Y = 10;
    const float SIZE_RANGE = 1.2f;
    const float MIN_SNOW_FLICKER = 0.1f;
    const float MAX_SNOW_FLICKER = 0.6f;
    const float MIN_SPEED = 0.5f;
    const float MAX_SPEED = 1.5f;

    Random rand = new Random();
    List<Snow> snowList = new List<Snow>();

    public void Initialize()
    {
        for (int i = 0; i < SNOW_COUNT; i++)
            snowList.Add(new Snow(
                GetRandomPos(MAP_SIZE_X, MAP_SIZE_Y),
                (float)(rand.NextDouble() * SIZE_RANGE) + 1.0f,
                (float)(rand.NextDouble() * (MAX_SPEED - MIN_SPEED) + MIN_SPEED),
                new Vector3(MIN_SNOW_FLICKER, MAX_SNOW_FLICKER, (float)(rand.NextDouble() * MAX_SNOW_FLICKER))));

        PluginHandlers.Framework.Update += Update;
    }

    Vector2 GetRandomPos(float rangeX, float rangeY) => new Vector2((float)(rand.NextDouble() * rangeX), (float)(rand.NextDouble() * rangeY));

    public void DrawSnowMapped(ImDrawListPtr drawListPtr, Vector2 startingPoint, Vector2 endPoint)
    {
        if (!PluginLink.Configuration.allowSnow) return;
        for (int i = 0; i < snowList.Count; i++)
        {
            Snow snow = snowList[i];
            Vector2 oldPos = snow.Position;
            Vector2 newPos = new Vector2(Map(oldPos.X, 0, MAP_SIZE_X, startingPoint.X, endPoint.X), Map(oldPos.Y, 0, MAP_SIZE_Y, startingPoint.Y, endPoint.Y));
            drawListPtr.AddCircleFilled(newPos, snow.Size, GetColour(1, 1, 1, snow.Visibility));
        }
    }

    public void Update(IFramework framework)
    {
        for (int i = 0; i < snowList.Count; i++)
        {
            Snow snow = snowList[i];
            float delta = (float)framework.UpdateDelta.TotalSeconds;
            Vector2 newPos = snow.Position + new Vector2(0, delta * snow.Speed);
            if (newPos.Y > MAP_SIZE_Y) newPos.Y -= MAP_SIZE_Y;
            snow.SetPosition(newPos);
            snow.Flicker(delta);
        }
    }

    public void Dispose()
    {
        PluginHandlers.Framework.Update -= Update;
        snowList.Clear();
    }

    float Map(float value, float istart, float istop, float ostart, float ostop)
    {
        return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
    }

    uint GetColour(float r, float g, float b, float a) => GetColour(new Vector4(r, g, b, a));
    uint GetColour(Vector4 color)
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
