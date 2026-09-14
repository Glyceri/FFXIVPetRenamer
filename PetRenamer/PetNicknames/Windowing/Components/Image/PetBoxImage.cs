using Dalamud.Bindings.ImGui;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
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
    public static void DrawPet(IPetServices petServices, DalamudServices dalamudServices, Vector2 size, IPetSheetData? petSheetData, IPettableDatabaseEntry? forUser = null)
    {
        InternalDrawPet(petServices, dalamudServices, size, petSheetData, forUser);
        
        ImGui.InvisibleButton($"###PETINVIS_{WindowHandler.InternalCounter}", size);
    }
    
    private static void InternalDrawPet(IPetServices petServices, DalamudServices dalamudServices, Vector2 size, IPetSheetData? petSheetData, IPettableDatabaseEntry? forUser = null)
    {
        Vector2 position = ImGui.GetCursorScreenPos();
        
        if (petSheetData == null)
        {
            DrawQuestionBox(size, position, dalamudServices);

            return;
        }
        
        DrawPaperPlate(size, position);
            
        switch (petSheetData.Model.SkeletonType)
        {
            case SkeletonType.Minion:       DrawMinion(petServices, dalamudServices, size, position, petSheetData);    break;
            case SkeletonType.BattlePet:    DrawBattlePet(dalamudServices, size, position, petSheetData); break;
            case SkeletonType.BeastMaster:  DrawBeast(petServices, dalamudServices, size, position, petSheetData, forUser); break;
        }
        
        DrawName(petServices, dalamudServices, size, position, petSheetData, forUser);
    }
    
    public static void DrawQuestionBox(Vector2 size, Vector2 position, DalamudServices dalamudServices)
    {
        if (XBMIconHelper.BeastActionBlock == null)
        {
            return;
        }
        
        DrawPaperPlate(size, position);
        DrawActionBox(size, position, XBMIconHelper.BeastActionBlock, SearchImage.GetSearchTexture(dalamudServices).GetWrapOrEmpty());
    }
    
    private static void DrawBattlePet(DalamudServices dalamudServices, Vector2 size, Vector2 position, IPetSheetData petSheetData)
    {
        if (XBMIconHelper.ActionBlock == null)
        {
            return;
        }
        
        DrawActionBox(size, position, XBMIconHelper.ActionBlock, dalamudServices.TextureProvider.GetFromGameIcon(petSheetData.Icon).GetWrapOrEmpty());
        
        DrawBattlePetIcon(dalamudServices, size, position);
    }
    
    private static void DrawBeast(IPetServices petServices, DalamudServices dalamudServices, Vector2 size, Vector2 position, IPetSheetData petSheetData, IPettableDatabaseEntry? forUser)
    {
        if (petServices.Configuration.IconTypeBeast == Configuration.BeastIconType.Blob)
        {
            DrawBeastBlob(dalamudServices, size, position, petSheetData);
        }
        else
        {
            DrawBeastAction(dalamudServices, size, position, petSheetData);
        }
        
        DrawHorn(petServices, size, position, petSheetData, forUser);
    }
    
    private static void DrawBeastBlob(DalamudServices dalamudServices, Vector2 size, Vector2 position, IPetSheetData petSheetData)
    {
        if (XBMIconHelper.BlobImagePart == null)
        {
            return;
        }
        
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        
        ISharedImmediateTexture petWrap = dalamudServices.TextureProvider.GetFromGameIcon(petSheetData.Icon);
       
        Vector2 insetScale = new Vector2(16);
        
        ImScaler.CreateScale(insetScale, size, position, out Vector2 usedInsetScale, out Vector2 usedInsetPos);
        
        windowDrawList.AddImage(XBMIconHelper.BlobImagePart.Handle, position, position + size, Vector2.Zero, Vector2.One);
        windowDrawList.AddImage(petWrap.GetWrapOrEmpty().Handle, usedInsetPos, usedInsetPos + usedInsetScale, Vector2.Zero, Vector2.One);
    }
    
    private static void DrawBeastAction(DalamudServices dalamudServices, Vector2 size, Vector2 position, IPetSheetData petSheetData)
    {
        if (XBMIconHelper.BeastActionBlock == null)
        {
            return;
        }
        
        DrawActionBox(size, position, XBMIconHelper.BeastActionBlock, dalamudServices.TextureProvider.GetFromGameIcon(petSheetData.Icon).GetWrapOrEmpty());
    }
    
    private static void DrawName(IPetServices petServices, DalamudServices dalamudServices, Vector2 size, Vector2 basePosition, IPetSheetData data, IPettableDatabaseEntry? forUser)
    {
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        
        if (forUser == null)
        {
            return;
        }
        
        if (XBMIconHelper.Button == null)
        {
            return;
        }
        
        string? customName = forUser.GetName(data.Model);
        
        customName ??= data.Singular;
        
        if (customName.IsNullOrWhitespace())
        {
            return;
        }
        
        size *= new Vector2(1.15f, 1);
        
        float width          = size.X * 0.7f;
        float remainderWidth = size.X * 0.1f;
        float height         = 32 * WindowHandler.GlobalScale;
        
        MakeTextDotted(customName, width - remainderWidth, out string textToUse, out float outWidth);
        
        Vector3? edgeColour = forUser.GetEdgeColour(data.Model);
        Vector3? textColour = forUser.GetTextColour(data.Model);
        SeString text       = petServices.StringHelper.WrapInColor(textToUse, edgeColour, textColour);
        
        Vector2 newPosition = basePosition + new Vector2((size.X - width) * 0.25f, size.Y - height - (5 * WindowHandler.GlobalScale));
        
        windowDrawList.AddImage(XBMIconHelper.Button.Handle, newPosition, newPosition + new Vector2(width, height), Vector2.Zero, Vector2.One);
        
        Vector2 currentPos = ImGui.GetCursorScreenPos();
        
        ImGui.SetCursorScreenPos(newPosition + new Vector2(width * 0.5f - outWidth * 0.5f, 5 * WindowHandler.GlobalScale));
        
        ImGuiHelpers.SeStringWrapped(text.EncodeWithNullTerminator());

        ImGui.SetCursorScreenPos(currentPos);
    }
    
    private static void MakeTextDotted(string input, float maxWidth, out string output, out float outWidth)
    {
        output   = input;
        outWidth = maxWidth;
        
        if (input.Length == 0)
        {
            return;
        }
        
        Vector2 textSize = ImGui.CalcTextSize(input);
        
        if (textSize.X < maxWidth)
        {
            output   = input;
            outWidth = textSize.X;
            
            return;
        }
        
        for (int i = input.Length - 1; i > 0; i--)
        {
            string newString = input.Substring(0, i);
            
            newString += "...";
            
            Vector2 newTextSize = ImGui.CalcTextSize(newString);
            
            if (newTextSize.X < maxWidth)
            {
                output   = newString;
                outWidth = newTextSize.X;
            
                return;
            }
        }
    }
    
    private static void DrawMinionIcon(Vector2 size, Vector2 basePosition)
    {
        if (XBMIconHelper.MinionIcon == null)
        {
            return;
        }
        
        DrawIcon(XBMIconHelper.MinionIcon, basePosition, size, 10);
    }
    
    private static void DrawBattlePetIcon(DalamudServices dalamudServices, Vector2 size, Vector2 basePosition)
    {
        ISharedImmediateTexture iconThing = dalamudServices.TextureProvider.GetFromGameIcon(62045);
        
        DrawIcon(iconThing.GetWrapOrEmpty(), basePosition, size, 10);
    }
    
    private static void DrawHorn(IPetServices petServices, Vector2 size, Vector2 basePosition, IPetSheetData data, IPettableDatabaseEntry? forUser)
    {
        if (forUser == null)
        {
            return;
        }
        
        IPettableUser? localPlayer = petServices.UserList.LocalPlayer;
        
        if (localPlayer == null)
        {
            return;
        }
        
        if (localPlayer.DataBaseEntry.ContentId != forUser.ContentId)
        {
            return;
        }
        
        int hornIndex = -1;
        
        for (byte i = 0; i < IHornService.AmountOfSlots; i++)
        {
            if (!petServices.HornService.TryGetPetForSlot(i, out XBMPet? pet))
            {
                continue;
            }
            
            if (data.Icon != pet.Value.Icon)
            {
                continue;
            }
            
            hornIndex = i;
            
            break;
        }
        
        IDalamudTextureWrap? horn = XBMIconHelper.GetHornTexture(hornIndex);
        
        if (horn == null)
        {
            horn = XBMIconHelper.XBMEmpty;
        }
        
        if (horn == null)
        {
            return;
        }
        
        DrawIcon(horn, basePosition, size, 12);
    }
    
    private static void DrawIcon(IDalamudTextureWrap texture, Vector2 basePosition, Vector2 size, int inset)
    {
        size *= 0.45f;
        basePosition += new Vector2(-10, -10) * WindowHandler.GlobalScale;
        
        Vector2 insetScale = new Vector2(inset);
        
        ImScaler.CreateScale(insetScale, size, basePosition, out Vector2 usedInsetScale, out Vector2 usedInsetPos);
        
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        
        windowDrawList.AddImage(texture.Handle, usedInsetPos, usedInsetPos + usedInsetScale, Vector2.Zero, Vector2.One);
    }
    
    private static void DrawMinion(IPetServices petServices, DalamudServices dalamudServices, Vector2 size, Vector2 position, IPetSheetData petSheetData)
    {
        if (petServices.Configuration.IconTypeMinion == Configuration.MinionIconType.Notebook)
        {
            DrawMinionNotebook(dalamudServices, size, position, petSheetData);
        }
        else
        {
            uint adder = 0;
            
            if (petServices.Configuration.IconTypeMinion == Configuration.MinionIconType.Item)
            {
                adder = 55000;
            }
            
            DrawMinionOther(dalamudServices, size, position, petSheetData.Icon + adder);  
        }
        
        DrawMinionIcon(size, position);
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
        
        Vector2 iconSize       = new Vector2(usedInsetScale.X * 0.2f, usedInsetScale.Y * 0.2f);
        Vector2 calculation    = new Vector2(usedInsetScale.X * 0.698f, usedInsetScale.Y * 0.005f);
        Vector2 offsetPosition = usedInsetPos + calculation;
        
        windowDrawList.AddImage(raceIcon.Handle, offsetPosition, offsetPosition + iconSize, Vector2.Zero, Vector2.One);
    }
    
    private static void DrawMinionOther(DalamudServices dalamudServices, Vector2 size, Vector2 position, uint iconId)
    {
        if (XBMIconHelper.ActionBlock == null)
        {
            return;
        }
        
        DrawActionBox(size, position, XBMIconHelper.ActionBlock, dalamudServices.TextureProvider.GetFromGameIcon(iconId).GetWrapOrEmpty());
    }
    
    private static void DrawActionBox(Vector2 size, Vector2 position, IDalamudTextureWrap customBoxWrap, IDalamudTextureWrap? internalDrawElement)
    {
        if (XBMIconHelper.ActionBacker == null)
        {
            return;
        }
        
        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        
        Vector2 borderImageSize = new Vector2(48, 48);
        Vector2 petImageSize    = new Vector2(40, 40);
        Vector2 scaledImageSize = ImScaler.MappedScale(borderImageSize, petImageSize);
        Vector2 insetScale      = new Vector2(8);
        
        ImScaler.CreateScale(insetScale, size, position, out Vector2 usedInsetScale, out Vector2 usedInsetPos);
        
        Vector4 colour = new Vector4(104, 96, 84, 102) / new Vector4(255);
        
        windowDrawList.AddImage(XBMIconHelper.ActionBacker.Handle, usedInsetPos, usedInsetPos + usedInsetScale, Vector2.Zero, Vector2.One, ImGui.GetColorU32(colour));
        
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
        if (XBMIconHelper.TopLeft == null || XBMIconHelper.TopRight == null || XBMIconHelper.BottomLeft == null || XBMIconHelper.BottomRight == null)
        {
            return;
        }
        
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