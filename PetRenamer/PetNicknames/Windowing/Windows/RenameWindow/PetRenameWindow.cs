using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using PetRenamer.PetNicknames.Windowing.Enums;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.TempWindow;

internal partial class PetRenameWindow : PetWindow
{
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPetServices PetServices;

    protected override Vector2 MinSize { get; } = new Vector2(550, 190);
    protected override Vector2 MaxSize { get; } = new Vector2(1500, 190);
    protected override Vector2 DefaultSize { get; } = new Vector2(550, 190);
    protected override bool HasModeToggle { get; } = true;

    protected override string Title { get; } = "Pet Nicknames";
    protected override string ID { get; } = "PetRenameWindow";

    PetRenameNode? petRenameNode;

    IPettableUser? ActiveUser;
    int activeSkeleton = 0;
    string? lastCustomName = null!;

    public PetRenameWindow(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database) : base(dalamudServices, "Pet Rename Window")
    {
        UserList = userList;
        Database = database;
        PetServices = petServices;
    }

    public unsafe override void OnDraw()
    {
        ActiveUser = UserList.LocalPlayer;
        if (ActiveUser == null || activeSkeleton == -1) 
        { 
            CleanOldNode();
        }
        if (ActiveUser != null)
        {
            if (ActiveUser.IsDirty)
            {
                GetActiveSkeleton();
            }
        }
    }

    public override void OnOpen() => GetActiveSkeleton();

    public void SetRenameWindow(int newSkeleton, bool openWindow = false)
    {
        activeSkeleton = newSkeleton;
        if (openWindow) IsOpen = true;
        if (activeSkeleton >= -1) SetPetMode(PetWindowMode.Minion);
        else SetPetMode(PetWindowMode.BattlePet);
    }

    protected override void OnPetModeChanged(PetWindowMode mode) => GetActiveSkeleton();

    void GetActiveSkeleton()
    {
        if (ActiveUser == null) return;
        IPettablePet? pet = ActiveUser.GetYoungestPet(CurrentMode == PetWindowMode.Minion ? IPettableUser.PetFilter.Minion : IPettableUser.PetFilter.BattlePet);
        if (pet == null)
        {
            activeSkeleton = -1;
            return;
        }
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

        if (petRenameNode == null)
        {
            AddNode(Node, petRenameNode = new PetRenameNode(customName, in data, in DalamudServices));
            petRenameNode.OnSave += OnSave;
        }
        else 
        {
            petRenameNode.Setup(customName, in data);
        }
    }

    void CleanOldNode()
    {
        if (petRenameNode == null) return;
        RemoveNode(Node, petRenameNode);
        petRenameNode.OnSave -= OnSave;
        petRenameNode?.Dispose();
        petRenameNode = null;
    }

    void OnSave(string? newName) => ActiveUser?.DataBaseEntry?.SetName(activeSkeleton, newName ?? "");

    protected sealed override Node Node { get; } = new() { };
}
