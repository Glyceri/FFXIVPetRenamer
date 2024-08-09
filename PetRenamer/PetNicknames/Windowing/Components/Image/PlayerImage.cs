using Dalamud.Game.Text;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using ImGuiNET;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Image;

internal static class PlayerImage
{
    public static void Draw(IPettableDatabaseEntry? entry, in IImageDatabase imageDatabase)
    {
        IDalamudTextureWrap? tWrap = imageDatabase.GetWrapFor(entry);
        if (tWrap == null) return;

        ImGuiStylePtr stylePtr = ImGui.GetStyle();
        float framePaddingX = stylePtr.FramePadding.X;
        float framePaddingY = stylePtr.FramePadding.Y;

        float size = ImGui.GetContentRegionAvail().Y;

        IconImage.Draw(tWrap, new Vector2(size, size));

        if (entry == null) return;

        ImGui.BeginDisabled(imageDatabase.IsBeingDownloaded(entry));

        Vector2 buttonSize = new Vector2(24, 24);

        ImGui.SameLine(0, 0);
        ImGui.SetCursorPos(ImGui.GetCursorPos() - new Vector2(buttonSize.X * ImGuiHelpers.GlobalScale + framePaddingX, -(size - buttonSize.Y - framePaddingY)));
        if (ImGui.Button(SeIconChar.QuestSync.ToIconString() + $"##RedownloadButton_{WindowHandler.InternalCounter}", buttonSize))
        {
            imageDatabase.Redownload(entry);
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Redownload profile Picture");
        }

        ImGui.EndDisabled();
    }
}
