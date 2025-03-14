﻿using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using ImGuiNET;
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
using PetRenamer.PetNicknames.Windowing.Enums;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetRenameWindow : PetWindow
{
    readonly IPettableUserList UserList;
    readonly IPetServices PetServices;
    readonly IPettableDirtyListener DirtyListener;

    protected override Vector2 MinSize { get; } = new Vector2(570, 250);
    protected override Vector2 MaxSize { get; } = new Vector2(1500, 250);
    protected override Vector2 DefaultSize { get; } = new Vector2(570, 250);

    protected override bool HasModeToggle { get; } = true;

    IPettableUser? ActiveUser;
    ulong lastContentID = 0;
    int activeSkeleton = -1;
    string? lastCustomName = null!;
    bool isContextOpen = false;

    string ActiveCustomName = string.Empty;
    string ActualCustomName = string.Empty;
    Vector3? ActiveEdgeColour = null;
    Vector3? ActiveTextColour = null;
    IPetSheetData? ActivePetData;
    ISharedImmediateTexture? ActivePetTexture;

    float BarHeight => 30 * ImGuiHelpers.GlobalScaleSafe;

    public PetRenameWindow(WindowHandler windowHandler, DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) : base(windowHandler, dalamudServices, petServices.Configuration, "Pet Rename Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        UserList = userList;
        PetServices = petServices;
        DirtyListener = dirtyListener;

        DirtyListener.RegisterOnPlayerCharacterDirty(DirtyPlayerChar);
    }

    protected override void OnDispose()
    {
        DirtyListener.UnregisterOnPlayerCharacterDirty(DirtyPlayerChar);
    }

    public void SetRenameWindow(int forSkeleton, bool open)
    {
        activeSkeleton = forSkeleton;
        if (open) IsOpen = true;
        if (forSkeleton >= -1) SetPetMode(PetWindowMode.Minion);
        else SetPetMode(PetWindowMode.BattlePet);
        isContextOpen = true;
        activeSkeleton = forSkeleton;
        ActiveUser = UserList.LocalPlayer;
        SetNewNode();
    }

    public override void OnOpen() 
    {
        if (isContextOpen) return;
        OnDraw();
        GetActiveSkeleton();
    }

    void DirtyPlayerChar(IPettableUser user)
    {
        if (user != ActiveUser) return;

        OnDirty();
    }

    protected override void OnDirty()
    {
        if (isContextOpen)
        {
            SetNewNode();
            return;
        }
        GetActiveSkeleton();
    }

    protected override void OnModeChange()
    {
        GetActiveSkeleton();
    }

    protected override void OnDraw()
    {
        Handle();
        DrawElement();
    }

    void Handle()
    {
        ActiveUser = UserList.LocalPlayer;

        if (lastContentID != ActiveUser?.ContentID)
        {
            lastContentID = ActiveUser?.ContentID ?? 0;
            isContextOpen = false;
            GetActiveSkeleton();
        }
    }

    void GetActiveSkeleton()
    {
        if (ActiveUser == null)
        {
            CleanOldNode();
            return;
        }
        IPettablePet? pet = ActiveUser.GetYoungestPet(CurrentMode == PetWindowMode.Minion ? IPettableUser.PetFilter.Minion : IPettableUser.PetFilter.BattlePet);
        if (pet == null)
        {
            activeSkeleton = -1;
            CleanOldNode();
            return;
        }
        SetPet(pet);
    }

    void SetPet(IPettablePet pet)
    {
        if (ActiveUser == null) return;

        bool dirty = activeSkeleton != pet.SkeletonID;
        string? customName = ActiveUser.DataBaseEntry.GetName(activeSkeleton);
        if (lastCustomName != customName)
        {
            lastCustomName = customName;
            dirty = true;
        }

        activeSkeleton = pet.SkeletonID;
        if (dirty) SetNewNode();
    }

    void SetNewNode()
    {
        if (!IsOpen) return;
        if (ActiveUser == null) return;
        if (activeSkeleton == -1) return;

        IPetSheetData? data = PetServices.PetSheets.GetPet(activeSkeleton);
        if (data == null) return;

        string? customName = ActiveUser.DataBaseEntry.GetName(activeSkeleton);
        lastCustomName = customName;

        Vector3? edgeColour = ActiveUser.DataBaseEntry.GetEdgeColour(activeSkeleton);
        Vector3? textColour = ActiveUser.DataBaseEntry.GetTextColour(activeSkeleton);

        Setup(customName, edgeColour, textColour, in data);
    }

    void CleanOldNode() => Setup(null, null, null, null);
    void Setup(string? customName, Vector3? edgeColour, Vector3? textColour, in IPetSheetData? petData)
    {
        ActiveCustomName = customName ?? string.Empty;
        ActualCustomName = ActiveCustomName;
        ActiveEdgeColour = edgeColour;
        ActiveTextColour = textColour;
        ActivePetData = petData;
        if (petData != null)
        {
            ActivePetTexture = DalamudServices.TextureProvider.GetFromGameIcon(petData.Icon);
        }
        else
        {
            ActivePetTexture = null;
        }
    }

    void OnSave(string? newName, Vector3? edgeColour, Vector3? textColour)
    {
        ActiveUser?.DataBaseEntry?.SetName(activeSkeleton, newName ?? "", edgeColour, textColour);
    }

    void DrawElement()
    {
        ImGuiStylePtr stylePtr = ImGui.GetStyle();
        float framePaddingX = stylePtr.ItemSpacing.X;

        Vector2 region = ImGui.GetContentRegionAvail();
        float regionHeight = region.Y;

        if (Listbox.Begin("##RenameHolder", ImGui.GetContentRegionAvail() - new Vector2(regionHeight + framePaddingX, 0)))
        {
            DrawInternals();
            Listbox.End();
        }

        ImGui.SameLine();

        DrawImageInternals(regionHeight);
    }

    void DrawInternals()
    {
        if (ActivePetData == null)
        {
            CenteredLabel.Draw(Translator.GetLine("PetRenameNode.PleaseSummonWarning"), new Vector2(ImGui.GetContentRegionAvail().X * 0.8f, BarHeight));
        }
        else
        {
            DrawPetData();
        }
    }

    void DrawPetData()
    {
        Vector2 contentSpot = new Vector2(ImGui.GetContentRegionAvail().X, BarHeight);

        LabledLabel.Draw($"{Translator.GetLine(CurrentMode == PetWindowMode.Minion ? "PetRenameNode.Species" : "PetRenameNode.Species2")}:", ActivePetData?.BaseSingular ?? Translator.GetLine("..."), contentSpot);
        LabledLabel.Draw("ID:", ActivePetData?.Model.ToString() ?? Translator.GetLine("..."), contentSpot);
        LabledLabel.Draw($"{Translator.GetLine("PetRenameNode.Race")}:", ActivePetData?.RaceName ?? Translator.GetLine("..."), contentSpot);
        LabledLabel.Draw($"{Translator.GetLine("PetRenameNode.Behaviour")}:", ActivePetData?.BehaviourName ?? Translator.GetLine("..."), contentSpot);

        if (RenameLabel.Draw($"{Translator.GetLine("PetRenameNode.Nickname")}:", ActiveCustomName == ActualCustomName, ref ActiveCustomName, ref ActiveEdgeColour, ref ActiveTextColour, contentSpot))
        {
            OnSave(ActiveCustomName, ActiveEdgeColour, ActiveTextColour);
        }

        ActiveCustomName = ActiveCustomName.Replace("\n", string.Empty);
    }

    void DrawImageInternals(float regionHeight)
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
                BoxedImage.DrawMinion(ActivePetData, in DalamudServices, in Configuration, ImGui.GetContentRegionAvail());

                Listbox.End();
            }
        }
    }
}
