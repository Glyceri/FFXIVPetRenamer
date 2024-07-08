using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using Una.Drawing;
using System.Numerics;
using Dalamud.Interface.Textures;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class BackgroundNode : Node
{
    ISharedImmediateTexture BGTexture;

    public BackgroundNode(in DalamudServices dalamudServices, uint iconID)
    {
        BGTexture = dalamudServices.TextureProvider.GetFromGameIcon(iconID);
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        base.OnDraw(drawList);
        Rect contentRect = Bounds.ContentRect;

        float zero = 0;
        float one = 1;

        Vector2 max = contentRect.TopLeft + new Vector2(1480, 840);

        float X = Map(contentRect.BottomRight.X, contentRect.TopLeft.X, max.X, zero, one);
        float Y = Map(contentRect.BottomRight.Y, contentRect.TopLeft.Y, max.Y, zero, one);

        Vector2 uv = new Vector2(zero, zero);
        Vector2 uv2 = new Vector2(X, zero);
        Vector2 uv3 = new Vector2(X, Y);
        Vector2 uv4 = new Vector2(zero, Y);
        drawList.AddImageQuad(BGTexture.GetWrapOrEmpty().ImGuiHandle, contentRect.TopLeft, contentRect.TopRight, contentRect.BottomRight, contentRect.BottomLeft, uv, uv2, uv3, uv4, new Color(255, 255, 0, 255).ToUInt());
    }

    float Map(float value, float istart, float istop, float ostart, float ostop) => ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
}
