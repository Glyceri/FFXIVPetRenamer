using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ContextMenus.ContextMenuElements;
using PetRenamer.PetNicknames.ContextMenus.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookTypes;
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
    private readonly DalamudServices            DalamudServices;
    private readonly IPetServices               PetServices;
    private readonly IPettableUserList          UserList;
    private readonly IWindowHandler             WindowHandler;
    private readonly IActionTooltipHook         ActionTooltipHook;
    private readonly List<IContextMenuElement>  ContextMenuElements = [];

    public ContextMenuHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList userList, IWindowHandler windowHandler, IActionTooltipHook actionTooltipHook)
    {
        DalamudServices   = dalamudServices;
        WindowHandler     = windowHandler;
        PetServices       = petServices;
        UserList          = userList;
        ActionTooltipHook = actionTooltipHook;

        DalamudServices.ContextMenu.OnMenuOpened += OnOpenMenu;

        _Register();
    }

    private void _Register()
    {
        Register(new TargetContextMenu(PetServices, UserList, WindowHandler));
        Register(new MinionNoteBookContextMenu(UserList, WindowHandler, ActionTooltipHook));
        Register(new MJIMinionNotebookContextMenu(UserList, WindowHandler, ActionTooltipHook));
        Register(new QuickPanelContextMenu(UserList, WindowHandler, ActionTooltipHook));
    }

    private void Register(IContextMenuElement contextMenuElement)
    {
        ContextMenuElements.Add(contextMenuElement);
    }

    private void OnOpenMenu(IMenuOpenedArgs args)
    {
        PetServices.PetLog.LogVerbose($"Tried to open the context menu for: '{args.AddonName}'.");

        if (!PetServices.Configuration.useContextMenus)
        {
            return;
        }

        foreach(IContextMenuElement contextMenuElement in ContextMenuElements)
        {
            if (contextMenuElement.AddonName != args.AddonName)
            {
                continue;
            }

            Action<IMenuItemClickedArgs>? callback = contextMenuElement.OnOpenMenu(args);

            if (callback == null)
            {
                continue;
            }

            PetServices.PetLog.LogVerbose($"Pet Nicknames registered a contextmenu callback for: '{args.AddonName}' that came from: '{contextMenuElement.GetType().Name}'.");

            RegisterCallback(args, callback);
        }
    }

    private void RegisterCallback(IMenuOpenedArgs args, Action<IMenuItemClickedArgs> callback)
    {
        args.AddMenuItem(new MenuItem()
        {
            Name        = Translator.GetLine("ContextMenu.Rename"),
            Prefix      = SeIconChar.BoxedLetterP,
            PrefixColor = 0,
            OnClicked   = callback
        });
    }

    public void Dispose()
    {
        DalamudServices.ContextMenu.OnMenuOpened -= OnOpenMenu;
    }
}
