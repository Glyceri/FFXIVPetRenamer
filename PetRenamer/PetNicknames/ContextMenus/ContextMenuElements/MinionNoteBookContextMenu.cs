using Dalamud.Game.Gui.ContextMenu;
using PetRenamer.PetNicknames.ContextMenus.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookElements;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.TempWindow;
using System;

namespace PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;

internal class MinionNoteBookContextMenu : IContextMenuElement
{
    public string? AddonName { get; } = "MinionNoteBook";

    readonly IPettableUserList UserList;
    readonly IWindowHandler WindowHandler;
    readonly IActionTooltipHook ActionTooltipHook;
    readonly IPetSheets PetSheets;

    public MinionNoteBookContextMenu(in IPetSheets petSheets, in IPettableUserList userList, in IWindowHandler windowHandler, in IActionTooltipHook actionTooltipHook)
    {
        PetSheets = petSheets;
        UserList = userList;
        WindowHandler = windowHandler;
        ActionTooltipHook = actionTooltipHook;
    }

    public Action<IMenuItemClickedArgs>? OnOpenMenu(IMenuOpenedArgs args)
    {
        IPettableUser? localUser = UserList.LocalPlayer;
        if (localUser == null) return null;

        IPetSheetData? petData = PetSheets.GetPetFromAction(ActionTooltipHook.LastActionID, localUser, false);
        if (petData == null) return null;

        return (a) => WindowHandler.GetWindow<PetRenameWindow>()?.SetRenameWindow(petData.Model, true);
    }
}
