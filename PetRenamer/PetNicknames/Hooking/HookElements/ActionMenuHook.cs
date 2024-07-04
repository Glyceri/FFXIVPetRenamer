using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkComponentList;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class ActionMenuHook : HookableElement
{
    public ActionMenuHook(DalamudServices services, IPetServices petServices, IPettableUserList userList) : base(services, userList, petServices) { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "ActionMenu", LifeCycleUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRefresh, "ActionMenu", LifeCycleUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "ActionMenuReplaceList", LifeCycleUpdate2);
    }

    void LifeCycleUpdate(AddonEvent addonEvent, AddonArgs addonArgs) => Update((AtkUnitBase*)addonArgs.Addon);
    void LifeCycleUpdate2(AddonEvent addonEvent, AddonArgs addonArgs) => Update2((AtkUnitBase*)addonArgs.Addon);

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
            Rename(tNode, ref user);
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
            Rename(tNode, ref user);
        }
    }

    void Rename(AtkTextNode* textNode, ref IPettableUser user)
    {
        string textNodeText = textNode->NodeText.ToString();
        string baseString = textNodeText.Split('\r')[0];
        string cleanedString = PetServices.StringHelper.CleanupString(baseString);

        List<PetSheetData> petSheetList = PetServices.PetSheets.GetListFromLine(baseString);
        if (petSheetList.Count == 0) return;

        petSheetList.Sort((i1, i2) => i1.BasePlural.CompareTo(i2.BasePlural));
        petSheetList.Reverse();

        PetSheetData petData = GetSoftData(petSheetList[0], cleanedString, ref user);

        string? customName = user.DataBaseEntry.GetName(petData.Model);
        if (customName == null) return;

        PetServices.StringHelper.ReplaceATKString(textNode, textNodeText, customName, petData);
    }

    PetSheetData GetSoftData(PetSheetData normalPetData, string cleanedString, ref IPettableUser user)
    {
        int? softIndex = PetServices.PetSheets.NameToSoftSkeletonIndex(cleanedString);
        if (softIndex == null) return normalPetData;

        int? softSkeleton = user.DataBaseEntry.GetSoftSkeleton(softIndex.Value);
        if (softSkeleton == null) return normalPetData;

        PetSheetData? softPetData = PetServices.PetSheets.GetPet(softSkeleton.Value);
        if (softPetData == null) return normalPetData;

        DalamudServices services = DalamudServices;

        return new PetSheetData(softPetData.Value.Model, softPetData.Value.Icon, softPetData.Value.Pronoun, normalPetData.BaseSingular, normalPetData.BasePlural, ref services);
    }

    public override void Dispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate2);
    }
}
