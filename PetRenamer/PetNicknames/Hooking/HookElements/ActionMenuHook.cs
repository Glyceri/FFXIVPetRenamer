using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkComponentList;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class ActionMenuHook : HookableElement
{
    public ActionMenuHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) : base(services, userList, petServices, dirtyListener) { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "ActionMenu", LifeCycleUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, "ActionMenu", LifeCycleUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "ActionMenu", LifeCycleUpdate);
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

        AtkComponentBase* resNode = baseD->GetComponentNodeById(6)->Component;
        if (resNode == null) return;

        AtkTextNode* textNode = resNode->GetTextNodeById(10)->GetAsAtkTextNode();
        if (textNode == null) return;

        ushort nodeCount = resNode->UldManager.NodeListCount;
        if (nodeCount < 6) return;

        AtkComponentIcon* icon = (AtkComponentIcon*)resNode->UldManager.NodeList[5]->GetAsAtkComponentNode()->Component;
        if (icon == null) return;

        long iconID = icon->IconId;

        AtkComponentBase* resNode2 = baseD->GetComponentNodeById(11)->Component;
        if (resNode2 == null) return;

        AtkTextNode* textNode2 = resNode2->GetTextNodeById(10)->GetAsAtkTextNode();
        if (textNode2 == null) return;

        ushort nodeCount2 = resNode2->UldManager.NodeListCount;
        if (nodeCount2 < 6) return;

        AtkComponentIcon* icon2 = (AtkComponentIcon*)resNode2->UldManager.NodeList[5]->GetAsAtkComponentNode()->Component;
        if (icon2 == null) return;

        long iconID2 = icon2->IconId;

        Rename(textNode, in user, iconID);
        Rename(textNode2, in user, iconID2);
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
            if (!tNode->IsVisible()) continue;

            ushort nodelistCount = cBase.UldManager.NodeListCount;
            if (nodelistCount < 5) continue;

            AtkComponentIcon* icon = (AtkComponentIcon*)cBase.UldManager.NodeList[4]->GetAsAtkComponentNode()->Component;
            if (icon == null) continue;

            long iconID = icon->IconId;

            Rename(tNode, in user, iconID);
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
            if (!node->IsVisible()) continue;
            if (node->Component == null) continue;
            if (node->Component->UldManager.NodeListCount != 11) continue;

            AtkComponentBase* atkNode = node->Component;
            if (atkNode == null) continue;

            if (TryAsDragAndDropNode(atkNode, in user)) continue;
            if (TryAsFlatNode(atkNode, in user)) continue;
        }
    }

    bool TryAsDragAndDropNode(AtkComponentBase* atkNode, in IPettableUser user)
    {
        AtkTextNode* tNode = (AtkTextNode*)atkNode->GetTextNodeById(10);
        if (tNode == null) return false;
        if (!tNode->IsVisible()) return false;

        ushort count = atkNode->UldManager.NodeListCount;
        if (count < 7) return false;

        AtkComponentNode* dragDropNode = atkNode->UldManager.NodeList[6]->GetAsAtkComponentNode();
        if (dragDropNode == null) return false;

        AtkComponentBase* dragDropBase = dragDropNode->Component;
        if (dragDropBase == null) return false;

        ushort count2 = dragDropBase->UldManager.NodeListCount;
        if (count2 < 2) return false;

        AtkComponentNode* iconBaseNode = (AtkComponentNode*)dragDropBase->UldManager.NodeList[2]->GetAsAtkComponentNode();
        if (iconBaseNode == null) return false;

        AtkComponentIcon* iconNode = (AtkComponentIcon*)iconBaseNode->Component;
        if (iconNode == null) return false;

        long iconID = iconNode->IconId;
        if (iconID <= 0 || iconID > 50000) return false;

        Rename(tNode, in user, iconID);
        return true;
    }

    bool TryAsFlatNode(AtkComponentBase* atkNode, in IPettableUser user)
    {
        AtkTextNode* tNode = (AtkTextNode*)atkNode->GetTextNodeById(10);
        if (tNode == null) return false;
        if (!tNode->IsVisible()) return false;

        ushort count = atkNode->UldManager.NodeListCount;
        if (count < 6) return false;

        AtkComponentNode* iconBaseNode = (AtkComponentNode*)atkNode->UldManager.NodeList[5]->GetAsAtkComponentNode();
        if (iconBaseNode == null) return false;

        AtkComponentIcon* iconNode = (AtkComponentIcon*)iconBaseNode->Component;
        if (iconNode == null) return false;

        long iconID = iconNode->IconId;
        if (iconID == 0) return false;

        Rename(tNode, in user, iconID);
        return true;
    }

    void Rename(AtkTextNode* textNode, in IPettableUser user, long iconID)
    {
        string textNodeText = textNode->NodeText.ToString();

        IPetSheetData? petSheet = PetServices.PetSheets.GetPetFromIcon(iconID);
        if (petSheet == null) return;

        IPetSheetData? softData = PetServices.PetSheets.MakeSoft(in user, in petSheet);
        if (softData == null) return;

        string? customName = user.DataBaseEntry.GetName(softData.Model);
        if (customName == null) return;

        if (!PetServices.Configuration.showNamesInActionLog) return;

        PetServices.StringHelper.ReplaceATKString(textNode, textNodeText, customName, petSheet);
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate2);
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate3);
    }
}
