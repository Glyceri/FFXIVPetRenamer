using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows.TempWindow;

internal partial class TempWindow : PetWindow
{
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;

    protected override Vector2 MinSize { get; } = new Vector2(400, 190);
    protected override Vector2 MaxSize { get; } = new Vector2(1500, 190);
    protected override Vector2 DefaultSize { get; } = new Vector2(400, 190);

    protected override string Title { get; } = "Pet Nicknames";
    protected override string ID { get; } = "PetRenameWindow";

    string newName = "";
    string newName2 = "";
    string tempSkeleton = "";

    public TempWindow(DalamudServices dalamudServices, IPettableUserList userList, IPettableDatabase database) : base(dalamudServices, "Pet Rename Window")
    {
        UserList = userList;
        Database = database;

        EnableModeToggle();
    }

    public unsafe override void OnDaw()
    {

    }
}
