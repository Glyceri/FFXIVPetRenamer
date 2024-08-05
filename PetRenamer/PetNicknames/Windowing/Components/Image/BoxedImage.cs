using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Image;

internal static class BoxedImage
{
    public static void Draw(IDalamudTextureWrap textureWrap, Vector2 size)
    {
        if (Listbox.Begin("##image", size))
        {
            ImGuiStylePtr style = ImGui.GetStyle();
            ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(style.FramePadding.X, style.FramePadding.Y));
            IconImage.Draw(textureWrap, ImGui.GetContentRegionAvail() - style.FramePadding);
            Listbox.End();
        }
    }
}
