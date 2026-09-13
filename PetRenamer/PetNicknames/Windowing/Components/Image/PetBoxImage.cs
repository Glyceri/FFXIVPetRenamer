using Dalamud.Bindings.ImGui;
using Dalamud.Interface.ImGuiSeStringRenderer;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Components.Image.UldHelpers;
using System;
using System.Numerics;
using XBMPet = PetRenamer.PetNicknames.Services.ServiceWrappers.Sheets.XBMPetButActuallyWorkingSinceForSomeReasonWePutInUnsusedSheetsAndNowEverythingIsABreakingChangeBecauseWhyWouldntItBeLikeWhatAreWeGenuinelyDoingHere;

namespace PetRenamer.PetNicknames.Windowing.Components.Image;

internal static class PetBoxImage
{
    public static void DrawPet(IPetServices petServices, DalamudServices dalamudServices, Vector2 size, IPetSheetData? petSheetData, IPettableUser? forUser = null)
    {
        Vector2 position = ImGui.GetCursorScreenPos();
        
        DrawPaperPlate(size, position);
        
        if (petSheetData == null)
        {
            DrawActionBox(size, position, XBMIconHelper.BeastActionBlock, SearchImage.GetSearchTexture(dalamudServices).GetWrapOrEmpty());
            
            return;
        }

        switch (petSheetData.Model.SkeletonType)
        {
            case SkeletonType.Minion:       DrawMinion(petServices, dalamudServices, size, position, petSheetData);    break;
            case SkeletonType.BattlePet:    DrawBattlePet(dalamudServices, size, position, petSheetData); break;
            case SkeletonType.BeastMaster:  DrawBeast(petServices, dalamudServices, size, position, petSheetData, forUser); break;
        }
    }
    
    private static void DrawBattlePet(DalamudServices dalamudServices, Vector2 size, Vector2 position, IPetSheetData petSheetData)
    {
        DrawActionBox(size, position, XBMIconHelper.ActionBlock, dalamudServices.TextureProvider.GetFromGameIcon(petSheetData.Icon).GetWrapOrEmpty());
    }
    
    private static void DrawBeast(IPetServices petServices, DalamudServices dalamudServices, Vector2 size, Vector2 position, IPetSheetData petSheetData, IPettableUser? forUser)
    {
        if (petServices.Configuration.IconTypeBeast == Configuration.BeastIconType.Blob)
        {
            DrawBeastBlob(dalamudServices, size, position, petSheetData);
        }
        else
        {
            DrawBeastAction(dalamudServices, size, position, petSheetData);
        }
    }
    
    private static void DrawBeastBlob(DalamudServices dalamudServices, Vector2 size, Vector2 position, IPetSheetData petSheetData)
    {
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        
        ISharedImmediateTexture petWrap = dalamudServices.TextureProvider.GetFromGameIcon(petSheetData.Icon);
       
        Vector2 insetScale = new Vector2(16);
        
        ImScaler.CreateScale(insetScale, size, position, out Vector2 usedInsetScale, out Vector2 usedInsetPos);
        
        windowDrawList.AddImage(XBMIconHelper.BlobImagePart.Handle, position, position + size, Vector2.Zero, Vector2.One);
        windowDrawList.AddImage(petWrap.GetWrapOrEmpty().Handle, usedInsetPos, usedInsetPos + usedInsetScale, Vector2.Zero, Vector2.One);
        
        
    }
    
    private static void DrawBeastAction(DalamudServices dalamudServices, Vector2 size, Vector2 position, IPetSheetData petSheetData)
    {
        DrawActionBox(size, position, XBMIconHelper.BeastActionBlock, dalamudServices.TextureProvider.GetFromGameIcon(petSheetData.Icon).GetWrapOrEmpty());

    }
    
    private static void DrawMinion(IPetServices petServices, DalamudServices dalamudServices, Vector2 size, Vector2 position, IPetSheetData petSheetData)
    {
        if (petServices.Configuration.IconTypeMinion == Configuration.MinionIconType.Notebook)
        {
            DrawMinionNotebook(dalamudServices, size, position, petSheetData);
        
            return;
        }
        
        uint adder = 0;
        
        if (petServices.Configuration.IconTypeMinion == Configuration.MinionIconType.Item)
        {
            adder = 55000;
        }
        
        DrawMinionOther(dalamudServices, size, position, petSheetData.Icon + adder);  
    }
    
    private static void DrawMinionNotebook(DalamudServices dalamudServices, Vector2 size, Vector2 position, IPetSheetData data)
    {
        Vector2 insetScale = new Vector2(10);
        
        ImScaler.CreateScale(insetScale, size, position, out Vector2 usedInsetScale, out Vector2 usedInsetPos);
        
        IDalamudTextureWrap iconWrap = dalamudServices.TextureProvider.GetFromGameIcon(data.Icon + 64000).GetWrapOrEmpty();
        
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        
        windowDrawList.AddImage(iconWrap.Handle, usedInsetPos, usedInsetPos + usedInsetScale, Vector2.Zero, Vector2.One);
        
        IDalamudTextureWrap? raceIcon = RaceIconHelper.GetFromRaceId(data.RaceId);
        
        if (raceIcon == null)
        {
            return;
        }
        
        Vector2 iconSize       = new Vector2(size.X * 0.193f, size.Y * 0.191f);
        Vector2 calculation    = new Vector2(iconSize.X * 3.17f, 0);
        Vector2 offsetPosition = usedInsetPos + calculation;
        
        windowDrawList.AddImage(raceIcon.Handle, offsetPosition, offsetPosition + iconSize, Vector2.Zero, Vector2.One);
    }
    
    private static void DrawMinionOther(DalamudServices dalamudServices, Vector2 size, Vector2 position, uint iconId)
    {
        DrawActionBox(size, position, XBMIconHelper.ActionBlock, dalamudServices.TextureProvider.GetFromGameIcon(iconId).GetWrapOrEmpty());
    }
    
    private static void DrawActionBox(Vector2 size, Vector2 position, IDalamudTextureWrap customBoxWrap, IDalamudTextureWrap? internalDrawElement)
    {
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        
        Vector2 borderImageSize = new Vector2(48, 48);
        Vector2 petImageSize    = new Vector2(40, 40);
        Vector2 scaledImageSize = ImScaler.MappedScale(borderImageSize, petImageSize);
        Vector2 insetScale      = new Vector2(8);
        
        ImScaler.CreateScale(insetScale, size, position, out Vector2 usedInsetScale, out Vector2 usedInsetPos);
        
        if (internalDrawElement != null)
        {
            Vector2 internalScale    = usedInsetScale * scaledImageSize;
            Vector2 internalPosition = usedInsetPos + (usedInsetScale - internalScale) * 0.5f;
            
            windowDrawList.AddImage(internalDrawElement.Handle, internalPosition, internalPosition + internalScale, Vector2.Zero, Vector2.One);
        }
        
        windowDrawList.AddImage(customBoxWrap.Handle, usedInsetPos, usedInsetPos + usedInsetScale, Vector2.Zero, Vector2.One);
    }
    
    public static void DrawPaperPlate(Vector2 size, Vector2 position)
    {
        Vector2 scaling = new Vector2(-12);
        
        IDalamudTextureWrap topLeft     = XBMIconHelper.TopLeft;
        IDalamudTextureWrap topRight    = XBMIconHelper.TopRight;
        IDalamudTextureWrap bottomLeft  = XBMIconHelper.BottomLeft;
        IDalamudTextureWrap bottomRight = XBMIconHelper.BottomRight;
        
        ImScaler.CreateScale(scaling, size, position, out Vector2 newScale, out Vector2 newPosition);

        Vector2 halfSize = Vector2.Round(newScale * 0.5f, MidpointRounding.ToNegativeInfinity);
        
        Vector2 tlx      = newPosition;
        Vector2 trx      = newPosition + new Vector2(halfSize.X, 0);
        Vector2 blx      = newPosition + new Vector2(0, halfSize.Y);
        Vector2 brx      = newPosition + halfSize;
        
        tlx = Vector2.Round(tlx, MidpointRounding.ToNegativeInfinity);
        trx = Vector2.Round(trx, MidpointRounding.ToNegativeInfinity);
        blx = Vector2.Round(blx, MidpointRounding.ToNegativeInfinity);
        brx = Vector2.Round(brx, MidpointRounding.ToNegativeInfinity);
        
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        
        windowDrawList.AddImage(topLeft.Handle,     tlx, tlx + halfSize, Vector2.Zero, new Vector2(1, 0.388888889f));
        windowDrawList.AddImage(topRight.Handle,    trx, trx + halfSize, Vector2.Zero, new Vector2(1, 0.388888889f));
        windowDrawList.AddImage(bottomLeft.Handle,  blx, blx + halfSize, Vector2.Zero, Vector2.One);
        windowDrawList.AddImage(bottomRight.Handle, brx, brx + halfSize, Vector2.Zero, Vector2.One);
    }
}