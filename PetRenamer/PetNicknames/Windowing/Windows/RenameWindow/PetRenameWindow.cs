using ImGuiNET;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using PetRenamer.PetNicknames.Windowing.Enums;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows.TempWindow;

internal partial class PetRenameWindow : PetWindow
{
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPetServices PetServices;

    protected override Vector2 MinSize { get; } = new Vector2(475, 170);
    protected override Vector2 MaxSize { get; } = new Vector2(475, 170);
    protected override Vector2 DefaultSize { get; } = new Vector2(475, 170);
    protected override bool HasModeToggle { get; } = true;

    protected override string Title { get; } = Translator.GetLine("PetRenameWindow.Title");
    protected override string ID { get; } = "PetRenameWindow";

    PetRenameNode? petRenameNode;

    IPettableUser? ActiveUser;
    IPettableUser? lastActiveUser;
    int activeSkeleton = 0;
    string? lastCustomName = null!;

    public PetRenameWindow(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database) : base(dalamudServices, "Pet Rename Window", ImGuiWindowFlags.NoResize)
    {
        UserList = userList;
        Database = database;
        PetServices = petServices;
        petRenameNode = new PetRenameNode(null, null, in DalamudServices);
        AddNode(ContentNode, petRenameNode);
        petRenameNode.OnSave += OnSave;
    }

    public unsafe override void OnDraw()
    {
        ActiveUser = UserList.LocalPlayer;
        if (ActiveUser?.IsDirty ?? false || lastActiveUser != ActiveUser)
        {
            lastActiveUser = ActiveUser;
            GetActiveSkeleton();
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

    void OnSave(string? newName) => ActiveUser?.DataBaseEntry?.SetName(activeSkeleton, newName ?? "");
}
