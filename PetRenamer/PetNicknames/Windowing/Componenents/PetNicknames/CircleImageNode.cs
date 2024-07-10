using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using System;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class CircleImageNode : Node
{
    ISharedImmediateTexture Texture;

    const float topLeftX = 0.128f;
    const float topLeftY = 0.58f;

    const float botRightX = 0.328f;
    const float botRightY = 0.937f;

    float Rotation { get; set; } = 0;


    public float RoationSpeed { get; set; } = 0;
    public float Opacity { get; set; } = 1;

    DateTime StartTime;

    public CircleImageNode(in DalamudServices dalamudServices)
    {
        Texture = dalamudServices.TextureProvider.GetFromGameIcon(195007);
        StartTime = DateTime.Now;
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        DateTime now = DateTime.Now;
        TimeSpan time = StartTime - now;
        StartTime = now;
        float elapsed = (float)time.TotalSeconds;

        Rotation += RoationSpeed * elapsed;
        IDalamudTextureWrap wrap = Texture.GetWrapOrEmpty();

        float invertedResolution = (float)wrap.Width / (float)wrap.Height;

        Rect contentRect = Bounds.ContentRect;

        Vector2 centre = new Vector2(topLeftX * invertedResolution, topLeftY) + ((new Vector2(botRightX * invertedResolution, botRightY) - new Vector2(topLeftX * invertedResolution, topLeftY)) * 0.5f);

        Vector2 uv = new Vector2(topLeftX * invertedResolution, topLeftY);
        Vector2 uv2 = new Vector2(botRightX * invertedResolution, topLeftY);
        Vector2 uv3 = new Vector2(botRightX * invertedResolution, botRightY);
        Vector2 uv4 = new Vector2(topLeftX * invertedResolution, botRightY);

        uv = RotateAroundPoint(uv, centre, invertedResolution);
        uv2 = RotateAroundPoint(uv2, centre, invertedResolution);
        uv3 = RotateAroundPoint(uv3, centre, invertedResolution);
        uv4 = RotateAroundPoint(uv4, centre, invertedResolution);

        drawList.AddImageQuad(wrap.ImGuiHandle, contentRect.TopLeft, contentRect.TopRight, contentRect.BottomRight, contentRect.BottomLeft, uv, uv2, uv3, uv4, new Color(255, 255, 0, Opacity).ToUInt());
    }

    Vector2 RotateAroundPoint(Vector2 point, Vector2 centre, float vectorResolution)
    {
        float angleInrad = Rotation * (MathF.PI / 180.0f);
        float cos = MathF.Cos(angleInrad);
        float sin = MathF.Sin(angleInrad);

        return new Vector2((cos * (point.X - centre.X) - sin * (point.Y - centre.Y) + centre.X) / vectorResolution, 
                           (sin * (point.X - centre.X) + cos * (point.Y - centre.Y) + centre.Y));
    }
}
