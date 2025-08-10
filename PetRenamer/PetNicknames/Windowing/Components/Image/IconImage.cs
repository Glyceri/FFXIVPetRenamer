using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.Windowing.Components.Texture;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Image;

internal static class IconImage
{
    public static void Draw(IDalamudTextureWrap textureWrap, Vector2 size)
    {
        ImGui.Image(textureWrap.Handle, size);
    }

    public static void DrawUld(UldIcon icon, Vector2 size)
    {
        float texWidth = icon.Texture.Width;
        float texHeight = icon.Texture.Height;

        Vector2 uvmin = new Vector2(icon.Offset.X / texWidth, icon.Offset.Y / texHeight);
        Vector2 uvmax = new Vector2((icon.Offset.X + icon.Size.X) / texWidth, (icon.Offset.Y + icon.Size.Y) / texHeight);

        ImGui.Image(icon.Texture.Handle, size, uvmin, uvmax);
    }
}
