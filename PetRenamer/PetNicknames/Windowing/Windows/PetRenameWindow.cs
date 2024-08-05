using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Components;
using PetRenamer.PetNicknames.Windowing.Components.Image;
using PetRenamer.PetNicknames.Windowing.Enums;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class PetRenameWindow : PetWindow
{
    readonly IPettableUserList UserList;
    readonly IPetServices PetServices;

    protected override Vector2 MinSize { get; } = new Vector2(437, 192);
    protected override Vector2 MaxSize { get; } = new Vector2(1500, 392);
    protected override Vector2 DefaultSize { get; } = new Vector2(437, 192);

    protected override bool HasModeToggle { get; } = true;
    protected override bool HasExtraButtons { get; }

    IPettableUser? ActiveUser;
    ulong lastContentID = 0;
    int activeSkeleton = -1;
    string? lastCustomName = null!;
    bool isContextOpen = false;

    string? ActiveCustomName;
    IPetSheetData? ActivePetData;

    public PetRenameWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, IPetServices petServices, IPettableUserList userList) : base(windowHandler, dalamudServices, configuration, "Pet Rename Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        Open();

        UserList = userList;
        PetServices = petServices;
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

    protected override void OnDirty()
    {
        if (isContextOpen)
        {
            SetNewNode();
            return;
        }
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

        if (ActiveUser?.IsDirty ?? false || lastContentID != ActiveUser?.ContentID)
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

        Setup(customName, in data);
    }

    void CleanOldNode() => Setup(null, null);
    void Setup(string? customName, in IPetSheetData? petData)
    {
        ActiveCustomName = customName;
        ActivePetData = petData;
    }

    void OnSave(string? newName)
    {
        ActiveUser?.DataBaseEntry?.SetName(activeSkeleton, newName ?? "");
    }


    void DrawElement()
    {
        ImGuiStylePtr stylePtr = ImGui.GetStyle();
        float framePaddingX = stylePtr.ItemSpacing.X;

        Vector2 region = ImGui.GetContentRegionAvail();
        float regionHeight = region.Y;

        if (Listbox.Begin("##RenameHolder", ImGui.GetContentRegionAvail() - new Vector2(regionHeight + framePaddingX, 0)))
        {

            Listbox.End();
        }

        ImGui.SameLine();

        IDalamudTextureWrap? searchTexture = SearchImage.SearchTextureWrap;
        if (searchTexture != null)
        {
            BoxedImage.Draw(searchTexture, new Vector2(regionHeight, regionHeight));
        }
    }
}
