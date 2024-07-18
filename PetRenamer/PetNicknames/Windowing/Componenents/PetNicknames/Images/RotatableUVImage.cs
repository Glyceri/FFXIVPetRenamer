using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Textures;
using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using System;
using Una.Drawing;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Images;

internal class RotatableUVImage : Node
{
    ISharedImmediateTexture Texture;

    float topLeftX = 0.128f;
    float topLeftY = 0.58f;

    float botRightX = 0.328f;
    float botRightY = 0.937f;

    float Rotation { get; set; } = 0;

    public float RoationSpeed { get; set; } = 0;
    public float Opacity { get; set; } = 1;

    DateTime StartTime;

    public Vector3 Color { get; set; } = new Vector3(255, 255, 255);

    readonly Configuration Configuration;

    public RotatableUVImage(in DalamudServices dalamudServices, in Configuration configuration, uint icon, float topLeftX, float topLeftY, float botRightX, float botRightY)
    {
        Configuration = configuration;

        Texture = dalamudServices.TextureProvider.GetFromGameIcon(icon);
        StartTime = DateTime.Now;

        this.topLeftX = topLeftX;
        this.topLeftY = topLeftY;
        this.botRightX = botRightX;
        this.botRightY = botRightY;
    }

    public RotatableUVImage(in DalamudServices dalamudServices, in Configuration configuration, uint icon, Vector2 topLeft, Vector2 botRight) : this (in dalamudServices, in configuration, icon, topLeft.X, topLeft.Y, botRight.X, botRight.Y)
    {
       
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        if (!Configuration.uiFlare) return;

        DateTime now = DateTime.Now;
        TimeSpan time = StartTime - now;
        StartTime = now;
        float elapsed = (float)time.TotalSeconds;

        Rotation += RoationSpeed * elapsed;
        IDalamudTextureWrap wrap = Texture.GetWrapOrEmpty();

        float invertedResolution;
        Vector2 resolution;

        if (wrap.Width > wrap.Height)
        {
            invertedResolution = wrap.Width / (float)wrap.Height;
            resolution = new Vector2(invertedResolution, 1);
        }
        else
        {
            invertedResolution = wrap.Height / (float)wrap.Width;
            resolution = new Vector2(1, invertedResolution);
        }        


        Rect contentRect = Bounds.ContentRect;

        Vector2 topLeft = new Vector2(topLeftX, topLeftY);
        Vector2 topRight = new Vector2(botRightX, topLeftY);
        Vector2 botRight = new Vector2(botRightX, botRightY);
        Vector2 botLeft = new Vector2(topLeftX, botRightY);
        Vector2 centre = (topLeft + (botRight - topLeft) * 0.5f) * resolution;

        Vector2 uv = topLeft * resolution;
        Vector2 uv2 = topRight * resolution;
        Vector2 uv3 = botRight * resolution;
        Vector2 uv4 = botLeft * resolution;

        uv = RotateAroundPoint(uv, centre) / resolution;
        uv2 = RotateAroundPoint(uv2, centre) / resolution;
        uv3 = RotateAroundPoint(uv3, centre) / resolution;
        uv4 = RotateAroundPoint(uv4, centre) / resolution;

        drawList.AddImageQuad(wrap.ImGuiHandle, contentRect.TopLeft, contentRect.TopRight, contentRect.BottomRight, contentRect.BottomLeft, uv, uv2, uv3, uv4, new Color((byte)Color.X, (byte)Color.Y, (byte)Color.Z, Opacity).ToUInt());
    }

    Vector2 RotateAroundPoint(Vector2 point, Vector2 centre)
    {
        float angleInrad = Rotation * (MathF.PI / 180.0f);
        float cos = MathF.Cos(angleInrad);
        float sin = MathF.Sin(angleInrad);

        return new Vector2((cos * (point.X - centre.X) - sin * (point.Y - centre.Y) + centre.X),
                           sin * (point.X - centre.X) + cos * (point.Y - centre.Y) + centre.Y);
    }

}
