using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Components;
using PetRenamer.PetNicknames.Windowing.Components.Image;
using PetRenamer.PetNicknames.Windowing.Components.Labels;
using System.Numerics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using System;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetRenameWindow : PetWindow
{
    private PetSkeleton?             ActiveSkeleton;
    
    private string                   ActiveCustomName   = string.Empty;
    private string                   EditableCustomName = string.Empty;
    private Vector3?                 ActiveEdgeColour   = null;
    private Vector3?                 ActiveTextColour   = null;
    private IPetSheetData?           ActivePetData      = null;
    private ISharedImmediateTexture? ActivePetTexture   = null;

    public PetRenameWindow(WindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices) 
        : base(windowHandler, dalamudServices, petServices, "Pet Nicknames", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        PetServices.DirtyListener.RegisterOnDirtyPet(DirtyPet);
        PetServices.DirtyListener.RegisterOnPlayerCharacterDirty(DirtyUser);
        PetServices.DirtyListener.RegisterOnDirtyName(DirtyName);
    }

    protected override void OnDispose()
    {
        PetServices.DirtyListener.UnregisterOnDirtyName(DirtyName);
        PetServices.DirtyListener.UnregisterOnDirtyPet(DirtyPet);
        PetServices.DirtyListener.UnregisterOnPlayerCharacterDirty(DirtyUser);
    }

    public override bool HasModeToggle 
        => true;
    
    public override bool ShowQuickButtons
        => true;

    protected override Vector2 MinSize  
        => new Vector2(570, 210);
    
    protected override Vector2 MaxSize 
        => new Vector2(1500, 210);
    
    protected override Vector2 DefaultSize
        => MinSize;
    
    public void SetRenameWindow(PetSkeleton forSkeleton)
    {
        switch (forSkeleton.SkeletonType)
        {
            case SkeletonType.Minion:      SetPetMode(SkeletonType.Minion);      break;
            case SkeletonType.BattlePet:   SetPetMode(SkeletonType.BattlePet);   break;
            case SkeletonType.BeastMaster: SetPetMode(SkeletonType.BeastMaster); break;
        }
        
        IsOpen         = true;
        ActiveSkeleton = forSkeleton;

        Refresh();
    }

    public override void OnOpen() 
    {
        if (ActiveSkeleton != null)
        {
            return;
        }
        
        GetActiveSkeleton();
    }

    private void DirtyName(INamesDatabase nameDatabase)
    {
        if (ActiveSkeleton == null)
        {
            return;
        }
        
        string? name = nameDatabase.GetName(ActiveSkeleton.Value);
        
        if (name == ActiveCustomName)
        {
            return;
        }
        
        Refresh();
    }
    
    private void DirtyUser(IPettableUser user)
    {
        if (!user.IsLocalPlayer)
        {
            return;
        }
        
        Refresh();
    }
    
    private void DirtyPet(IPettablePet pet)
    {
        if (ActiveSkeleton != null)
        {
            return;
        }
        
        if (pet.Owner == null)
        {
            return;
        }
        
        if (!pet.Owner.IsLocalPlayer)
        {
            return;
        }

        ActiveSkeleton = pet.SkeletonId;
        
        Refresh();
    }

    protected override void OnModeChange()
    {
        GetActiveSkeleton();
    }

    protected override void OnDraw()
    {
        DrawElement();
    }

    private void GetActiveSkeleton()
    {
        ActiveSkeleton = null;
        
        CleanOldNode();
        
        if (PetServices.UserList.LocalPlayer == null)
        {
            return;
        }
        
        IPettablePet? pet = PetServices.UserList.LocalPlayer.GetYoungestPet(PetMode);
        
        if (pet == null)
        {
            return;
        }
        
        ActiveSkeleton = pet.SkeletonId;
        
        Refresh();
    }
    
    private void Refresh()
    {
        CleanOldNode();
        
        if (!IsOpen)
        {
            return;
        }
        
        if (PetServices.UserList.LocalPlayer == null)
        {
            return;
        }

        if (ActiveSkeleton == null)
        {
            return;
        }
        
        IPetSheetData? data = PetServices.PetSheets.GetPet(ActiveSkeleton.Value);

        if (data == null)
        {
            return;
        }
        
        string? customName  = PetServices.UserList.LocalPlayer.DataBaseEntry.GetName(ActiveSkeleton.Value);
        
        Vector3? edgeColour = PetServices.UserList.LocalPlayer.DataBaseEntry.GetEdgeColour(ActiveSkeleton.Value);
        Vector3? textColour = PetServices.UserList.LocalPlayer.DataBaseEntry.GetTextColour(ActiveSkeleton.Value);

        Setup(customName, edgeColour, textColour, data);
    }

    private void CleanOldNode() 
        => Setup(null, null, null, null);

    private void Setup(string? customName, Vector3? edgeColour, Vector3? textColour, IPetSheetData? petData)
    {
        ActiveCustomName   = customName ?? string.Empty;
        EditableCustomName = ActiveCustomName;
        ActiveEdgeColour   = edgeColour;
        ActiveTextColour   = textColour;
        ActivePetData      = petData;
        ActivePetTexture   = null;
        
        if (ActivePetData == null)
        {
            return;
        }
        
        ActivePetTexture = DalamudServices.TextureProvider.GetFromGameIcon(ActivePetData.Icon);
    }

    private void OnSave(PetSkeleton skeletonId, string? newName, Vector3? edgeColour, Vector3? textColour)
        => PetServices.UserList.LocalPlayer?.DataBaseEntry.SetName(skeletonId, newName ?? string.Empty, edgeColour, textColour);
    
    private void DrawElement()
    {
        ImGuiStylePtr stylePtr = ImGui.GetStyle();
        Vector2       region   = ImGui.GetContentRegionAvail();
        
        float   framePaddingX  = stylePtr.ItemSpacing.X;
        float   regionHeight   = region.Y;

        if (!Listbox.Begin("##RenameHolder", ImGui.GetContentRegionAvail() - new Vector2(regionHeight + framePaddingX, 0)))
        {
            return;
        }
        
        DrawInternals();

        Listbox.End();

        ImGui.SameLine();

        DrawImageInternals(regionHeight);
    }

    private void DrawInternals()
    {
        if (ActivePetData != null)
        {
            DrawPetData();
            
            return;
        }
        
        string summonWarningLine = Translator.GetLine("PetRenameNode.PleaseSummonWarning");
        string outputLine        = string.Format(summonWarningLine, SpeciesLine);
        
        CenteredLabel.Draw(outputLine, new Vector2(ImGui.GetContentRegionAvail().X * 0.8f, WindowHandler.BarHeight));
    }

    private void DrawPetData()
    {
        if (ActiveSkeleton == null)
        {
            return;
        }
        
        LabledLabel.Draw($"{SpeciesLine}:",                                   ActivePetData?.Singular         ?? Translator.GetLine("..."), WindowHandler.StretchingBar);
        LabledLabel.Draw($"{Translator.GetLine("PetRenameNode.Id")}:",        ActivePetData?.Model.ToString() ?? Translator.GetLine("..."), WindowHandler.StretchingBar);
        LabledLabel.Draw($"{Translator.GetLine("PetRenameNode.Race")}:",      ActivePetData?.RaceName         ?? Translator.GetLine("..."), WindowHandler.StretchingBar);
        LabledLabel.Draw($"{Translator.GetLine("PetRenameNode.Behaviour")}:", ActivePetData?.BehaviourName    ?? Translator.GetLine("..."), WindowHandler.StretchingBar);

        if (RenameLabel.Draw($"{Translator.GetLine("PetRenameNode.Nickname")}:", ActiveCustomName == EditableCustomName, ref EditableCustomName, ref ActiveEdgeColour, ref ActiveTextColour, WindowHandler.StretchingBar))
        {
            EditableCustomName = EditableCustomName.Replace(Environment.NewLine, string.Empty);
            
            OnSave(ActiveSkeleton.Value, EditableCustomName, ActiveEdgeColour, ActiveTextColour);
        }

        ActiveCustomName = ActiveCustomName.Replace(Environment.NewLine, string.Empty);
    }

    private void DrawImageInternals(float regionHeight)
    {
        Vector2 size = new Vector2(regionHeight, regionHeight);

        if (ActivePetTexture == null)
        {
            IDalamudTextureWrap? searchTexture = SearchImage.SearchTextureWrap;

            if (searchTexture != null)
            {
                BoxedImage.Draw(searchTexture, size);
            }
        }
        else if (ActivePetData != null)
        {
            if (Listbox.Begin("##image", size))
            {
                BoxedImage.DrawMinion(ActivePetData, DalamudServices, PetServices.Configuration, ImGui.GetContentRegionAvail());

                Listbox.End();
            }
        }
    }
}
