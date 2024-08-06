using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
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

    public static void DrawMinion(in IPetSheetData data, in DalamudServices dalamudServices, in Configuration configuration, Vector2 size)
    {
        IDalamudTextureWrap textureWrap;

        if (data.Model <= -1) 
        {
            textureWrap = dalamudServices.TextureProvider.GetFromGameIcon(data.Icon).GetWrapOrEmpty(); 
        }
        else
        {
            uint adder = 0;

            if (configuration.minionIconType == 1)
            {
                adder = 64000;
            }
            else if (configuration.minionIconType == 2)
            {
                adder = 55000;
            }

            textureWrap = dalamudServices.TextureProvider.GetFromGameIcon(data.Icon + adder).GetWrapOrEmpty();
        }

        IconImage.Draw(textureWrap, size);
    }
}
