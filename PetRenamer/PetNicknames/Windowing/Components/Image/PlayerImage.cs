using Dalamud.Game.Text;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
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
        if (ImGui.Button(SeIconChar.Cross.ToIconString() + $"##CancelRedownloadButton_{WindowHandler.InternalCounter}", buttonSize))
        {
            imageDatabase.Cancel(entry);
        }
        
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(Translator.GetLine("Download.Cancel"));
        }
    }
    
    public static void Draw(IPettableDatabaseEntry? entry, IImageDatabase imageDatabase, DalamudServices dalamudServices)
    {
        if (entry == null)
        {
            return;
        }
        
        IDalamudTextureWrap? tWrap = imageDatabase.GetWrapFor(entry);
        
        float   height    = ImGui.GetContentRegionAvail().Y;
        Vector2 size      = new Vector2(height);
        Vector2 smallSize = size * 0.85f;
        Vector2 pSize     = size * 0.8f;
        Vector2 offset    = (size - smallSize) * 0.5f;
        Vector2 pOffset   = (size - pSize) * 0.5f;
        Vector2 screenPos = ImGui.GetCursorScreenPos();
        Vector2 pPos      = screenPos + pOffset;
        Vector2 bSize     = size * 0.75f;
        
        if (tWrap == null)
        {
            PetBoxImage.DrawQuestionBox(smallSize, screenPos + offset, dalamudServices);
        }
        else
        {
            ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
            
            PetBoxImage.DrawPaperPlate(smallSize, screenPos + offset);
            
            windowDrawList.AddImage(tWrap.Handle, pPos, pPos + pSize, Vector2.Zero, Vector2.One);
        }
        
        Vector2 buttonSize = new Vector2(24, 24) * ImGuiHelpers.GlobalScale;
        
        ImGui.SetCursorScreenPos(screenPos + bSize);
        
        if (!imageDatabase.IsBeingDownloaded(entry))
        {
            DrawRedownload(buttonSize, entry, imageDatabase);
        }
        else
        {
            DrawCancel(buttonSize, entry, imageDatabase);
        }
        
        ImGui.SetCursorScreenPos(screenPos);
        
        ImGui.InvisibleButton($"###INVIS_{entry?.Name}", size);
    }
}
