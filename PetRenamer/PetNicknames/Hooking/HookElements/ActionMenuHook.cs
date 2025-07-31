using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using Lumina.Text.ReadOnly;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class ActionMenuHook : HookableElement
{
    public ActionMenuHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) : base(services, userList, petServices, dirtyListener) { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup,           "ActionMenu",               LifeCycleUpdateActionMenu);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh,         "ActionMenu",               LifeCycleUpdateActionMenu);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "ActionMenu",               LifeCycleUpdateActionMenu);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "ActionMenuReplaceList",    LifeCycleUpdateReplaceMenu);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup,           "ActionMenuActionSetting",  LifeCycleUpdateActionSetting);
    }

    void LifeCycleUpdateActionMenu    (AddonEvent addonEvent, AddonArgs addonArgs) => UpdateActionMenu    ((AtkUnitBase*)addonArgs.Addon);
    void LifeCycleUpdateReplaceMenu   (AddonEvent addonEvent, AddonArgs addonArgs) => UpdateReplaceMenu   ((AtkUnitBase*)addonArgs.Addon);
    void LifeCycleUpdateActionSetting (AddonEvent addonEvent, AddonArgs addonArgs) => UpdateActionSetting ((AtkUnitBase*)addonArgs.Addon);

    void UpdateActionSetting(AtkUnitBase* baseD)
    {
        if (baseD == null) return;
        if (!baseD->IsFullyLoaded()) return;
        if (!baseD->IsVisible) return;

        IPettableUser? user = UserList.LocalPlayer;
        if (user == null) return;
        if (!user.IsActive) return;

        AtkComponentNode* compNode = baseD->GetComponentNodeById(6);
        if (compNode == null) return;

        AtkComponentBase* resNode = compNode->Component;
        if (resNode == null) return;

        AtkResNode* textResNode = resNode->GetTextNodeById(10);
        if (textResNode == null) return;

        AtkTextNode* textNode = textResNode->GetAsAtkTextNode();
        if (textNode == null) return;

        if (resNode->UldManager.LoadedState != AtkLoadState.Loaded) return;
        if (resNode->UldManager.NodeList == null) return;

        ushort nodeCount = resNode->UldManager.NodeListCount;
        if (nodeCount < 6) return;

        AtkResNode* nodeListEl = resNode->UldManager.NodeList[5];
        if (nodeListEl == null) return;

        AtkComponentNode* iconCompNode = nodeListEl->GetAsAtkComponentNode();
        if (iconCompNode == null) return;

        AtkComponentIcon* icon = (AtkComponentIcon*)iconCompNode->Component;
        if (icon == null) return;

        uint iconID = icon->IconId;

        AtkComponentNode* resCompNode2 = baseD->GetComponentNodeById(11);
        if (resCompNode2 == null) return;

        AtkComponentBase* resNode2 = resCompNode2->Component;
        if (resNode2 == null) return;

        AtkTextNode* textNode2 = (AtkTextNode*)resNode2->GetTextNodeById(10);
        if (textNode2 == null) return;

        if (resNode2->UldManager.LoadedState != AtkLoadState.Loaded) return;
        if (resNode2->UldManager.NodeList == null) return;

        ushort nodeCount2 = resNode2->UldManager.NodeListCount;
        if (nodeCount2 < 6) return;

        AtkResNode* iconResNode = resNode2->UldManager.NodeList[5];
        if (iconResNode == null) return;

        AtkComponentNode* iconCompNode2 = iconResNode->GetAsAtkComponentNode();
        if (iconCompNode2 == null) return;

        AtkComponentIcon* icon2 = (AtkComponentIcon*)iconCompNode2->Component;
        if (icon2 == null) return;

        uint iconID2 = icon2->IconId;

        Rename(textNode, in user, iconID);
        Rename(textNode2, in user, iconID2);
    }

    void UpdateReplaceMenu(AtkUnitBase* baseD)
    {
        if (baseD == null) return;
        if (!baseD->IsVisible) return;

        AtkComponentList* list = baseD->GetComponentListById(3);
        if (list == null) return;
        if (list->ItemRendererList == null) return;

        IPettableUser? user = UserList.LocalPlayer;
        if (user == null) return;
        if (!user.IsActive) return;

        for (int i = 0; i < list->ListLength; i++)
        {
            AtkComponentListItemRenderer* renderer = list->ItemRendererList[i].AtkComponentListItemRenderer;
            if (renderer == null) continue;
            if (renderer->UldManager.LoadedState != AtkLoadState.Loaded) return;

            AtkTextNode* tNode = (AtkTextNode*)renderer->GetTextNodeById(4);
            if (tNode == null) continue;
            if (!tNode->IsVisible()) continue;

            ushort nodelistCount = renderer->UldManager.NodeListCount;
            if (nodelistCount < 5) continue;
            if (renderer->UldManager.NodeList == null) continue;

            AtkResNode* resNode = renderer->UldManager.NodeList[4];
            if (resNode == null) continue;

            AtkComponentNode* compNode = resNode->GetAsAtkComponentNode();
            if (compNode == null) continue;

            AtkComponentIcon* icon = (AtkComponentIcon*)compNode->Component;
            if (icon == null) continue;

            uint iconID = icon->IconId;

            Rename(tNode, in user, iconID);
        }
    }

    void UpdateActionMenu(AtkUnitBase* baseD)
    {
        if (baseD == null) return;
        if (!baseD->IsVisible) return;

        IPettableUser? user = UserList.LocalPlayer;
        if (user == null) return;
        if (!user.IsActive) return;
        if (baseD->UldManager.LoadedState != AtkLoadState.Loaded) return;
        if (baseD->UldManager.NodeList == null) return;

        for (int i = 0; i < baseD->UldManager.NodeListCount; i++)
        {
            AtkComponentNode* node = baseD->UldManager.NodeList[i]->GetAsAtkComponentNode();
            if (node == null) continue;
            if (node->Component == null) continue;
            if (node->Component->UldManager.LoadedState != AtkLoadState.Loaded) continue;
            if (node->Component->UldManager.NodeListCount != 11) continue;
            if (!node->IsVisible()) continue;

            AtkComponentBase* atkNode = node->Component;
            if (atkNode == null) continue;

            if (TryAsDragAndDropNode(atkNode, in user)) continue;
            if (TryAsFlatNode(atkNode, in user)) continue;
        }
    }

    bool TryAsDragAndDropNode(AtkComponentBase* atkNode, in IPettableUser user)
    {
        if (atkNode->UldManager.LoadedState != AtkLoadState.Loaded) return false;
        if (atkNode->UldManager.NodeList == null) return false;

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

        if (dragDropBase->UldManager.LoadedState != AtkLoadState.Loaded) return false;
        if (dragDropBase->UldManager.NodeList == null) return false;

        AtkComponentNode* iconBaseNode = dragDropBase->UldManager.NodeList[2]->GetAsAtkComponentNode();
        if (iconBaseNode == null) return false;

        AtkComponentIcon* iconNode = (AtkComponentIcon*)iconBaseNode->Component;
        if (iconNode == null) return false;

        uint iconID = iconNode->IconId;
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

        AtkComponentNode* iconBaseNode = atkNode->UldManager.NodeList[5]->GetAsAtkComponentNode();
        if (iconBaseNode == null) return false;

        AtkComponentIcon* iconNode = (AtkComponentIcon*)iconBaseNode->Component;
        if (iconNode == null) return false;

        uint iconID = iconNode->IconId;
        if (iconID == 0) return false;

        Rename(tNode, in user, iconID);
        return true;
    }

    void Rename(AtkTextNode* textNode, in IPettableUser user, uint iconID)
    {
        string textNodeText = new ReadOnlySeStringSpan(textNode->NodeText).ExtractText();

        IPetSheetData? petSheet = PetServices.PetSheets.GetPetFromIcon(iconID);
        if (petSheet == null) return;

        IPetSheetData? softData = PetServices.PetSheets.MakeSoft(in user, in petSheet);
        if (softData == null) return;

        string? customName = user.DataBaseEntry.GetName(softData.Model);
        if (customName == null) return;

        if (!PetServices.Configuration.showNamesInActionLog) return;

        PetServices.StringHelper.ReplaceATKString(textNode, textNodeText, customName, null, null, petSheet);
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdateActionMenu);
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdateReplaceMenu);
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdateActionSetting);
    }
}
