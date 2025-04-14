using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonPartyList;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using System.Numerics;
using Lumina.Text.ReadOnly;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class PartyHook : HookableElement
{
    private bool hasRegisteredListener = false;

    public PartyHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) : base(services, userList, petServices, dirtyListener) { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup,           "_PartyList", LifeCycleUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_PartyList", LifeCycleUpdate);
    }

    protected override void Refresh()
    {
        if (hasRegisteredListener) return;

        hasRegisteredListener = true;

        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostDraw, "_PartyList", LifeCycleUpdateRefresh);
    }

    bool CanContinue(AtkUnitBase* baseD) => baseD != null && baseD->IsFullyLoaded() && baseD->IsVisible;

    void LifeCycleUpdate(AddonEvent aEvent, AddonArgs args) => Update((AtkUnitBase*)args.Addon);
    void LifeCycleUpdateRefresh(AddonEvent aEvent, AddonArgs args)
    {
        Update((AtkUnitBase*)args.Addon);

        hasRegisteredListener = false;
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdateRefresh);
    }

    void Update(AtkUnitBase* baseD)
    {
        if (!CanContinue(baseD)) return;

        SetPetname  ((AddonPartyList*)baseD);
        SetCastlist ((AddonPartyList*)baseD);
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdateRefresh);
    }

    void SetPetname(AddonPartyList* partyNode)
    {
        if (!PetServices.Configuration.showOnPartyList) return;

        IPettableUser? localPlayer = UserList.LocalPlayer;
        if (localPlayer == null) return;

        IPettablePet? pet = localPlayer.GetYoungestPet(IPettableUser.PetFilter.BattlePet);
        if (pet == null) return;

        IPetSheetData? petData = pet.PetData;
        if (petData == null) return;

        string? lastPetname = pet.CustomName;
        if (lastPetname == string.Empty || lastPetname == null) return;

        pet.GetDrawColours(out Vector3? edgeColour, out Vector3? textColour);

        PetServices.StringHelper.ReplaceATKString(partyNode->Pet.Name, pet.Name, lastPetname, edgeColour, textColour, petData);
    }

    void SetCastlist(AddonPartyList* partyNode)
    {
        if (!PetServices.Configuration.showOnCastbars) return;
        SetupPartyList();

        int index = -1;
        foreach (PartyListMemberStruct member in partyNode->PartyMembers)
        {
            index++;
            if (index < 0 || index > partyGroup.Length) continue;

            if (member.Name == null) continue;
            if (member.CastingProgressBar == null) continue;
            if (!member.CastingProgressBar->AtkResNode.IsVisible()) continue;

            string castString = new ReadOnlySeStringSpan(member.CastingActionName->NodeText).ExtractText();
            if (castString == string.Empty) continue;

            ulong contentID = partyGroup[index];
            if (contentID == 0) // this means there is no party active
            {
                IPettableUser? localUser = UserList.LocalPlayer;
                if (localUser == null) return;

                contentID = localUser.ContentID;
            }

            IPettableUser? user = UserList.GetUserFromContentID(contentID);
            if (user == null) continue;

            IPetSheetData? data = PetServices.PetSheets.GetPetFromAction(user.CurrentCastID, in user, true);
            if (data == null) continue;

            string? customName = user.DataBaseEntry.GetName(data.Model);
            if (customName == null) continue;

            PetServices.StringHelper.ReplaceATKString(member.CastingActionName, castString, customName, null, null, data, false);
        }
    }

    ulong[] partyGroup = new ulong[8]; // a party is always 8 in size

    void SetupPartyList()
    {
        GroupManager* gManager = (GroupManager*)DalamudServices.PartyList.GroupManagerAddress;
        if (gManager == null) return;

        bool isCrossParty = IsCrossParty();
        partyGroup = new ulong[8];

        // We can assume partyGroup is 8 in length, and so is the struct with party members, Thanks SE!
        for (int i = 0; i < partyGroup.Length; i++)
        {
            PartyMember member = gManager->MainGroup.PartyMembers[i];
            ulong contentID = member.ContentId;

            int? index;

            if (isCrossParty) index = GetCrossPartyIndex(contentID);
            else index = GetNormalPartyIndex(contentID);

            if (index == null) continue;
            if (index < 0 || index >= partyGroup.Length) continue;

            partyGroup[index.Value] = contentID;
        }
    }

    bool IsCrossParty()
    {
        bool isCrossRealm       = InfoProxyCrossRealm.Instance()->IsCrossRealm   > 0;
        bool noMembersInGroup   = GroupManager.Instance()->MainGroup.MemberCount < 1;

        return isCrossRealm && noMembersInGroup;
    }

    int? GetCrossPartyIndex(ulong contentID)
    {
        if (InfoProxyCrossRealm.Instance() == null) return null;

        CrossRealmMember* member = InfoProxyCrossRealm.GetMemberByContentId(contentID);
        if (member == null) return null;

        return member->MemberIndex;
    }

    int? GetNormalPartyIndex(ulong contentID)
    {
        int memberCount = GroupManager.Instance()->MainGroup.MemberCount;
        bool foundSelf = false;

        ulong localContentID = UserList.LocalPlayer?.ContentID ?? 0;

        for (int i = 0; i < memberCount; i++)
        {
            int actualCurrent = i;
            if (!foundSelf)
            {
                actualCurrent++;
            }

            PartyMember* member = GroupManager.Instance()->MainGroup.GetPartyMemberByIndex(i);
            if (member == null) continue;

            if (member->ContentId == localContentID)
            {
                foundSelf = true;
                actualCurrent = 0;
            }

            if (contentID == member->ContentId)
            {
                return actualCurrent;
            }
        }

        return null;
    }
}
