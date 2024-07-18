using ImGuiNET;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.WindowNodes;
using PetRenamer.PetNicknames.Windowing.Enums;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows.TempWindow;

internal partial class PetRenameWindow : PetWindow
{
    readonly IPettableUserList UserList;
    readonly IPetServices PetServices;

    protected override Vector2 MinSize { get; } = new Vector2(475, 170);
    protected override Vector2 MaxSize { get; } = new Vector2(475, 170);
    protected override Vector2 DefaultSize { get; } = new Vector2(475, 170);
    protected override bool HasModeToggle { get; } = true;
    protected override bool HasExtraButtons { get; } = true;

    protected override string Title { get; } = Translator.GetLine("WindowHandler.Title");
    protected override string ID { get; } = "WindowHandler";

    readonly PetRenameNode? petRenameNode;

    IPettableUser? ActiveUser;
    ulong lastContentID = 0;
    int activeSkeleton = -1;
    string? lastCustomName = null!;

    bool isContextOpen = false;

    public PetRenameWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, IPetServices petServices, IPettableUserList userList) : base(windowHandler, dalamudServices, configuration, "Pet Rename Window", ImGuiWindowFlags.NoResize)
    {
        UserList = userList;
        PetServices = petServices;
        petRenameNode = new PetRenameNode(null, null, in DalamudServices, in configuration);
        AddNode(ContentNode, petRenameNode);
        petRenameNode.OnSave += OnSave;
    }

    public override void OnDirty()
    {
        if (isContextOpen)
        {
            SetNewNode();
            return;
        }
        GetActiveSkeleton();
    }

    public unsafe override void OnDraw()
    {
        ActiveUser = UserList.LocalPlayer;

        if (ActiveUser?.IsDirty ?? false || lastContentID != ActiveUser?.ContentID)
        {
            lastContentID = ActiveUser?.ContentID ?? 0;
            isContextOpen = false;
            GetActiveSkeleton();
        }
    }

    public override void OnOpen()
    {
        if (isContextOpen) return;
        OnDraw();
        GetActiveSkeleton();
    }

    public void SetRenameWindow(int newSkeleton, bool openWindow = false)
    {
        activeSkeleton = newSkeleton;
        if (openWindow) IsOpen = true;
        if (newSkeleton >= -1) SetPetMode(PetWindowMode.Minion);
        else SetPetMode(PetWindowMode.BattlePet);
        isContextOpen = true;
        activeSkeleton = newSkeleton;
        ActiveUser = UserList.LocalPlayer;
        SetNewNode();
    }

    protected override void OnPetModeChanged(PetWindowMode mode)
    {
        GetActiveSkeleton();
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

        petRenameNode?.Setup(customName, in data);
    }

    void CleanOldNode()
    {
        petRenameNode?.Setup(null, null);
    }

    void OnSave(string? newName)
    {
        ActiveUser?.DataBaseEntry?.SetName(activeSkeleton, newName ?? "");
    }
}
