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

    public float Rotation { get; set; } = 0;

    public CircleImageNode(in DalamudServices dalamudServices)
    {
        Texture = dalamudServices.TextureProvider.GetFromGameIcon(195007);
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        Rotation++;
        base.OnDraw(drawList);
        IDalamudTextureWrap wrap = Texture.GetWrapOrEmpty();

        float invertedResolution = (float)wrap.Width / (float)wrap.Height;

        Rect contentRect = Bounds.ContentRect;

        Vector2 vectorResolution = new Vector2(invertedResolution, 1);

        Vector2 centre = (new Vector2(topLeftX, topLeftY) + ((new Vector2(botRightX, botRightY) - new Vector2(topLeftX, topLeftY)) * 0.5f)) * vectorResolution;

        Vector2 uv = new Vector2(topLeftX, topLeftY) * vectorResolution;
        Vector2 uv2 = new Vector2(botRightX, topLeftY) * vectorResolution;
        Vector2 uv3 = new Vector2(botRightX, botRightY) * vectorResolution;
        Vector2 uv4 = new Vector2(topLeftX, botRightY) * vectorResolution;

        uv = RotateAroundPoint(uv, centre) / vectorResolution;
        uv2 = RotateAroundPoint(uv2, centre) / vectorResolution;
        uv3 = RotateAroundPoint(uv3, centre) / vectorResolution;
        uv4 = RotateAroundPoint(uv4, centre) / vectorResolution;

        drawList.AddImageQuad(wrap.ImGuiHandle, contentRect.TopLeft, contentRect.TopRight, contentRect.BottomRight, contentRect.BottomLeft, uv, uv2, uv3, uv4, new Color(255, 255, 0, 255).ToUInt());
    }

    Vector2 RotateAroundPoint(Vector2 point, Vector2 centre)
    {
        float angleInrad = Rotation * (MathF.PI / 180.0f);
        float cos = MathF.Cos(angleInrad);
        float sin = MathF.Sin(angleInrad);

        return new Vector2(cos * (point.X - centre.X) - sin * (point.Y - centre.Y) + centre.X, 
                           sin * (point.X - centre.X) + cos * (point.Y - centre.Y) + centre.Y);
    }
}
