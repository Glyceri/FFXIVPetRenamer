using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Utilization.UtilsModule;
using System.Runtime.InteropServices;
using System;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkComponentList;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
public unsafe class ActionMenuHook : HookableElement
{
    internal override void OnInit()
    {
        PluginHandlers.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "ActionMenu", LifeCycleUpdate);
        PluginHandlers.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, "ActionMenuReplaceList", LifeCycleUpdate2);
    }

    void LifeCycleUpdate(AddonEvent addonEvent, AddonArgs addonArgs) => Update((AtkUnitBase*)addonArgs.Addon);
    void LifeCycleUpdate2(AddonEvent addonEvent, AddonArgs addonArgs) => Update2((AtkUnitBase*)addonArgs.Addon);

    void Update2(AtkUnitBase* baseD)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;

        string? name = Marshal.PtrToStringUTF8((IntPtr)baseD->Name);
        if (!baseD->IsVisible || name != "ActionMenuReplaceList")
            return;

        AtkComponentList* list = (AtkComponentList*)baseD->GetComponentListById(3);
        if (list == null) return;

        for(int i = 0; i < list->ListLength; i++)
        {
            ListItem lItem = list->ItemRendererList[i];
            AtkComponentListItemRenderer* renderer = lItem.AtkComponentListItemRenderer;
            if (renderer == null) continue;
            AtkComponentButton button = renderer->AtkComponentButton;
            AtkComponentBase cBase = button.AtkComponentBase;
            AtkTextNode* tNode = (AtkTextNode*)cBase.GetTextNodeById(4);
            if (tNode == null) continue;        
            (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(PluginLink.PettableUserHandler.LocalUser()!, tNode->NodeText.ToString(), true);
            StringUtils.instance.ReplaceAtkString(tNode, ref validNames);
        }
        return;
    }

    void Update(AtkUnitBase* baseD)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;

        string? name = Marshal.PtrToStringUTF8((IntPtr)baseD->Name);
        if (!baseD->IsVisible || name != "ActionMenu")
            return;

        for(int i = 0; i < baseD->UldManager.NodeListCount; i++)
        {
            AtkComponentNode* node = baseD->UldManager.NodeList[i]->GetAsAtkComponentNode();
            if (node == null) continue;
            if (node->Component == null) continue;
            if (node->Component->UldManager.NodeListCount != 9) continue;
            AtkTextNode* tNode = (AtkTextNode*)node->Component->GetTextNodeById(8);
            if (tNode == null) continue;
            (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(PluginLink.PettableUserHandler.LocalUser()!, tNode->NodeText.ToString(), true);
            StringUtils.instance.ReplaceAtkString(tNode, ref validNames);
        }

        return ;
    }
}
