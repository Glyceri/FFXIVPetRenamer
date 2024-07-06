using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows.TempWindow;

internal partial class TempWindow : PetWindow
{
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPetServices PetServices;

    protected override Vector2 MinSize { get; } = new Vector2(400, 190);
    protected override Vector2 MaxSize { get; } = new Vector2(1500, 190);
    protected override Vector2 DefaultSize { get; } = new Vector2(400, 190);
    protected override bool HasModeToggle { get; } = true;

    protected override string Title { get; } = "Pet Nicknames";
    protected override string ID { get; } = "PetRenameWindow";

    string newName = "";
    string newName2 = "";
    string tempSkeleton = "";

    PetRenameNode? petRenameNode;

    public TempWindow(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IPettableDatabase database) : base(dalamudServices, "Pet Rename Window")
    {
        UserList = userList;
        Database = database;
        PetServices = petServices;
    }

    public unsafe override void OnDaw()
    {
        if (petRenameNode == null)
        {
            IPettableUser? localPlayer = UserList.LocalPlayer;
            if (localPlayer == null) return;

            IPettablePet? localMinion = localPlayer.GetYoungestPet(IPettableUser.PetFilter.Minion);
            if (localMinion == null) return;

            IPetSheetData? sheetData = localMinion.PetData;
            if (sheetData == null) return;

            Node.AppendChild(petRenameNode = new PetRenameNode(in localPlayer, in sheetData));
        }
    }
}
