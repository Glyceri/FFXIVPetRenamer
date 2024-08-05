using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Image;

internal static class IconImage
{
    public static void Draw(IDalamudTextureWrap textureWrap, Vector2 size)
    {
        ImGui.Image(textureWrap.ImGuiHandle, size);
    }
}
