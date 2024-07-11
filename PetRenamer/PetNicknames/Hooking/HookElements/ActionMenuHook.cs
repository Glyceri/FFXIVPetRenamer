using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkComponentList;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class ActionMenuHook : HookableElement
{
    public ActionMenuHook(DalamudServices services, IPetServices petServices, IPettableUserList userList) : base(services, userList, petServices) { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "ActionMenu", LifeCycleUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, "ActionMenu", LifeCycleUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "ActionMenuReplaceList", LifeCycleUpdate2);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "ActionMenuActionSetting", LifeCycleUpdate3);
    }

    void LifeCycleUpdate(AddonEvent addonEvent, AddonArgs addonArgs) => Update((AtkUnitBase*)addonArgs.Addon);
    void LifeCycleUpdate2(AddonEvent addonEvent, AddonArgs addonArgs) => Update2((AtkUnitBase*)addonArgs.Addon);
    void LifeCycleUpdate3(AddonEvent addonEvent, AddonArgs addonArgs) => Update3((AtkUnitBase*)addonArgs.Addon);

    void Update3(AtkUnitBase* baseD)
    {
        if (!baseD->IsVisible) return;

        IPettableUser? user = UserList.LocalPlayer;
        if (user == null) return;
        if (!user.IsActive) return;

        AtkComponentBase* resNode = (AtkComponentBase*)baseD->GetComponentNodeById(6);
        if (resNode == null) return;

        AtkTextNode* textNode = resNode->GetTextNodeById(10)->GetAsAtkTextNode();
        if (textNode == null) return;

        AtkComponentBase* resNode2 = (AtkComponentBase*)baseD->GetComponentNodeById(11);
        if (resNode2 == null) return;

        AtkTextNode* textNode2 = resNode2->GetTextNodeById(10)->GetAsAtkTextNode();
        if (textNode2 == null) return;

        Rename(textNode, in user);
        Rename(textNode2, in user);
    }

    void Update2(AtkUnitBase* baseD)
    {
        if (!baseD->IsVisible) return;

        AtkComponentList* list = baseD->GetComponentListById(3);
        if (list == null) return;

        IPettableUser? user = UserList.LocalPlayer;
        if (user == null) return;
        if (!user.IsActive) return;

        for (int i = 0; i < list->ListLength; i++)
        {
            ListItem lItem = list->ItemRendererList[i];
            AtkComponentListItemRenderer* renderer = lItem.AtkComponentListItemRenderer;
            if (renderer == null) continue;
            AtkComponentButton button = renderer->AtkComponentButton;
            AtkComponentBase cBase = button.AtkComponentBase;
            AtkTextNode* tNode = (AtkTextNode*)cBase.GetTextNodeById(4);
            if (tNode == null) continue;
            Rename(tNode, in user);
        }
    }

    void Update(AtkUnitBase* baseD)
    {
        if (!baseD->IsVisible) return;

        IPettableUser? user = UserList.LocalPlayer;
        if (user == null) return;
        if (!user.IsActive) return;

        for (int i = 0; i < baseD->UldManager.NodeListCount; i++)
        {
            AtkComponentNode* node = baseD->UldManager.NodeList[i]->GetAsAtkComponentNode();
            if (node == null) continue;
            if (node->Component == null) continue;
            if (node->Component->UldManager.NodeListCount != 11) continue;
            AtkTextNode* tNode = (AtkTextNode*)node->Component->GetTextNodeById(10);
            if (tNode == null) continue;
            Rename(tNode, in user);
        }
    }

    void Rename(AtkTextNode* textNode, in IPettableUser user)
    {
        string textNodeText = textNode->NodeText.ToString();
        string baseString = textNodeText.Split('\r')[0];

        IPetSheetData? petSheet = PetServices.PetSheets.GetPetFromString(baseString, in user, true);
        if (petSheet == null) return;

        string? customName = user.DataBaseEntry.GetName(petSheet.Model);
        if (customName == null) return;

        PetServices.StringHelper.ReplaceATKString(textNode, textNodeText, customName, petSheet);
    }

    public override void Dispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate2);
    }
}
