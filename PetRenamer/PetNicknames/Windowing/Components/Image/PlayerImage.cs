using Dalamud.Game.Text;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Image;

internal static class PlayerImage
{
    private static void DrawRedownload(Vector2 buttonSize, IPettableDatabaseEntry entry, IImageDatabase imageDatabase)
    {
        if (ImGui.Button(SeIconChar.QuestSync.ToIconString() + $"##RedownloadButton_{WindowHandler.InternalCounter}", buttonSize))
        {
            imageDatabase.Redownload(entry);
        }
        
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(Translator.GetLine("Download.Redownload"));
        }
    }
    
    private static void DrawCancel(Vector2 buttonSize, IPettableDatabaseEntry entry, IImageDatabase imageDatabase)
    {
        if (ImGui.Button(SeIconChar.BoxedLetterX.ToIconString() + $"##CancelRedownloadButton_{WindowHandler.InternalCounter}", buttonSize))
        {
            imageDatabase.Cancel(entry);
        }
        
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(Translator.GetLine("Download.Cancel"));
        }
    }
    
    public static void Draw(IPettableDatabaseEntry? entry, IImageDatabase imageDatabase)
    {
        IDalamudTextureWrap? tWrap = imageDatabase.GetWrapFor(entry);

        if (tWrap == null)
        {
            return;
        }

        ImGuiStylePtr stylePtr = ImGui.GetStyle();

        float framePaddingX = stylePtr.FramePadding.X;
        float framePaddingY = stylePtr.FramePadding.Y;

        float size = ImGui.GetContentRegionAvail().Y;

        IconImage.Draw(tWrap, new Vector2(size, size));

        ImGui.SameLine();

        Vector2 finalCursorPos = ImGui.GetCursorPos();

        if (entry == null)
        {
            return;
        }

        Vector2 buttonSize = new Vector2(24, 24) * WindowHandler.GlobalScale;

        ImGui.SameLine(0, 0);
        ImGui.SetCursorPos(ImGui.GetCursorPos() - new Vector2(buttonSize.X + framePaddingX, -(size - buttonSize.Y - framePaddingY)));
        
        if (!imageDatabase.IsBeingDownloaded(entry))
        {
            DrawRedownload(buttonSize, entry, imageDatabase);
        }
        else
        {
            DrawCancel(buttonSize, entry, imageDatabase);
        }

        ImGui.SameLine(0, 0);

        ImGui.SetCursorPos(finalCursorPos);
    }
}
