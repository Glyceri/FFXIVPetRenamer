using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkComponentList;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
public unsafe class ActionMenuHook : HookableElement
{
    internal override void OnInit()
    {
        PluginHandlers.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, "ActionMenu", LifeCycleUpdate);
        PluginHandlers.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, "ActionMenuReplaceList", LifeCycleUpdate2);
    }

    void LifeCycleUpdate(AddonEvent addonEvent, AddonArgs addonArgs) => Update((AtkUnitBase*)addonArgs.Addon);
    void LifeCycleUpdate2(AddonEvent addonEvent, AddonArgs addonArgs) => Update2((AtkUnitBase*)addonArgs.Addon);

    void Update2(AtkUnitBase* baseD)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (!baseD->IsVisible) return;

        AtkComponentList* list = baseD->GetComponentListById(3);
        if (list == null) return;
        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        for (int i = 0; i < list->ListLength; i++)
        {
            ListItem lItem = list->ItemRendererList[i];
            AtkComponentListItemRenderer* renderer = lItem.AtkComponentListItemRenderer;
            if (renderer == null) continue;
            AtkComponentButton button = renderer->AtkComponentButton;
            AtkComponentBase cBase = button.AtkComponentBase;
            AtkTextNode* tNode = (AtkTextNode*)cBase.GetTextNodeById(4);
            if (tNode == null) continue;
            (int, string) currentName = PettableUserUtils.instance.GetNameRework(tNode->NodeText.ToString(), ref user, true);
            StringUtils.instance.ReplaceAtkString(tNode, currentName.Item2, user.SerializableUser.GetNameFor(currentName.Item1));
        }
        return;
    }

    void Update(AtkUnitBase* baseD)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (!baseD->IsVisible) return;

        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        for (int i = 0; i < baseD->UldManager.NodeListCount; i++)
        {
            AtkComponentNode* node = baseD->UldManager.NodeList[i]->GetAsAtkComponentNode();
            if (node == null) continue;
            if (node->Component == null) continue;
            if (node->Component->UldManager.NodeListCount != 9) continue;
            AtkTextNode* tNode = (AtkTextNode*)node->Component->GetTextNodeById(8);
            if (tNode == null) continue;
            (int, string) currentName = PettableUserUtils.instance.GetNameRework(tNode->NodeText.ToString(), ref user, true);
            StringUtils.instance.ReplaceAtkString(tNode, currentName.Item2, user.SerializableUser.GetNameFor(currentName.Item1));
        }

        return ;
    }

    internal override void OnDispose()
    {
        PluginHandlers.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
        PluginHandlers.AddonLifecycle.UnregisterListener(LifeCycleUpdate2);
    }
}
