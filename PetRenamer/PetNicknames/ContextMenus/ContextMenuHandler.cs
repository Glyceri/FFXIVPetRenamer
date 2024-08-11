using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;
using PetRenamer.PetNicknames.ContextMenus.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookElements;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ContextMenus;

internal class ContextMenuHandler : IDisposable
{
    readonly DalamudServices DalamudServices;
    readonly IPetServices PetServices;
    readonly IPettableUserList UserList;
    readonly IWindowHandler WindowHandler;
    readonly IActionTooltipHook ActionTooltipHook;

    readonly List<IContextMenuElement> ContextMenuElements = new List<IContextMenuElement>();

    public ContextMenuHandler(in DalamudServices dalamudServices, in IPetServices petServices, in IPettableUserList userList, in IWindowHandler windowHandler, in IActionTooltipHook actionTooltipHook)
    {
        DalamudServices = dalamudServices;
        WindowHandler = windowHandler;
        PetServices = petServices;
        UserList = userList;
        ActionTooltipHook = actionTooltipHook;

        DalamudServices.ContextMenu.OnMenuOpened += OnOpenMenu;

        _Register();
    }

    void _Register()
    {
        Register(new TargetContextMenu(in DalamudServices, in UserList, in WindowHandler));
        Register(new MinionNoteBookContextMenu(PetServices.PetSheets, in UserList, in WindowHandler, in ActionTooltipHook));
        Register(new MJIMinionNotebookContextMenu(PetServices.PetSheets, in UserList, in WindowHandler, in ActionTooltipHook));
    }

    void Register(IContextMenuElement contextMenuElement)
    {
        ContextMenuElements.Add(contextMenuElement);
    }

    void OnOpenMenu(IMenuOpenedArgs args)
    {
        if (!PetServices.Configuration.useContextMenus) return;

        foreach(IContextMenuElement contextMenuElement in ContextMenuElements)
        {
            if (contextMenuElement.AddonName != args.AddonName) continue;

            Action<IMenuItemClickedArgs>? callback = contextMenuElement.OnOpenMenu(args);
            if (callback == null) continue;

            RegisterCallback(args, callback);
        }
    }

    void RegisterCallback(IMenuOpenedArgs args, Action<IMenuItemClickedArgs> callback)
    {
        args.AddMenuItem(new MenuItem()
        {
            Name = Translator.GetLine("ContextMenu.Rename"),
            Prefix = SeIconChar.BoxedLetterP,
            PrefixColor = 0,
            OnClicked = callback
        });
    }

    public void Dispose()
    {
        DalamudServices.ContextMenu.OnMenuOpened -= OnOpenMenu;
    }
}
