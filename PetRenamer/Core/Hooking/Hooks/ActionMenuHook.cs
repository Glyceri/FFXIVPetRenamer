using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Utilization.UtilsModule;
using System.Runtime.InteropServices;
using System;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkComponentList;
using Dalamud.Plugin.Services;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
public unsafe class ActionMenuHook : HookableElement
{
    private Hook<Delegates.AddonUpdate>? addonupdatehook = null;

    AtkUnitBase* actionMenu;

    private Hook<Delegates.AddonUpdate>? addonupdatehookreplacelist = null;

    AtkUnitBase* actionMenuReplaceList;

    internal override void OnUpdate(IFramework framework)
    {
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        actionMenu = (AtkUnitBase*)PluginHandlers.GameGui.GetAddonByName("ActionMenu");
        actionMenuReplaceList = (AtkUnitBase*)PluginHandlers.GameGui.GetAddonByName("ActionMenuReplaceList");
        if (actionMenu != null) {
            addonupdatehook ??= PluginHandlers.Hooking.HookFromFunctionPointerVariable<Delegates.AddonUpdate>(new nint(actionMenu->AtkEventListener.vfunc[PluginConstants.AtkUnitBaseUpdateIndex]), Update);
            addonupdatehook?.Enable();
        }

        if (actionMenuReplaceList != null)
        {
            addonupdatehookreplacelist ??= PluginHandlers.Hooking.HookFromFunctionPointerVariable<Delegates.AddonUpdate>(new nint(actionMenuReplaceList->AtkEventListener.vfunc[PluginConstants.AtkUnitBaseUpdateIndex]), Update2);
            addonupdatehookreplacelist?.Enable();
        }
    }

    byte Update2(AtkUnitBase* baseD)
    {
        if (!PluginLink.Configuration.displayCustomNames) return addonupdatehookreplacelist!.Original(baseD);

        string? name = Marshal.PtrToStringUTF8((IntPtr)baseD->Name);
        if (!baseD->IsVisible || name != "ActionMenuReplaceList")
            return addonupdatehookreplacelist!.Original(baseD);

        AtkComponentList* list = (AtkComponentList*)baseD->GetComponentListById(3);
        if (list == null) return addonupdatehookreplacelist!.Original(baseD);

        for(int i = 0; i < list->ListLength; i++)
        {
            ListItem lItem = list->ItemRendererList[i];
            AtkComponentListItemRenderer* renderer = lItem.AtkComponentListItemRenderer;
            if (renderer == null) continue;
            AtkComponentButton button = renderer->AtkComponentButton;
            AtkComponentBase cBase = button.AtkComponentBase;
            AtkTextNode* tNode = (AtkTextNode*)cBase.GetTextNodeById(4);
            if (tNode == null) continue;
            (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(PluginLink.PettableUserHandler.LocalUser()!, tNode->NodeText.ToString());
            StringUtils.instance.ReplaceAtkString(tNode, validNames);
        }
        return addonupdatehookreplacelist!.Original(baseD);
    }

    byte Update(AtkUnitBase* baseD)
    {
        if (!PluginLink.Configuration.displayCustomNames) return addonupdatehook!.Original(baseD);

        string? name = Marshal.PtrToStringUTF8((IntPtr)baseD->Name);
        if (!baseD->IsVisible || name != "ActionMenu")
            return addonupdatehook!.Original(baseD);

        for(int i = 0; i < baseD->UldManager.NodeListCount; i++)
        {
            AtkComponentNode* node = baseD->UldManager.NodeList[i]->GetAsAtkComponentNode();
            if (node == null) continue;
            if (node->Component == null) continue;
            if (node->Component->UldManager.NodeListCount != 9) continue;
            AtkTextNode* tNode = (AtkTextNode*)node->Component->GetTextNodeById(8);
            if (tNode == null) continue;
            (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(PluginLink.PettableUserHandler.LocalUser()!, tNode->NodeText.ToString());
            StringUtils.instance.ReplaceAtkString(tNode, validNames);
        }

        return addonupdatehook!.Original(baseD);
    }

    internal override void OnDispose()
    {
        addonupdatehook?.Dispose();
        addonupdatehookreplacelist?.Dispose();
    }
}
