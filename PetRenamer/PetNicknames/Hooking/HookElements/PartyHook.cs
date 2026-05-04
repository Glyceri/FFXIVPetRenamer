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
using Lumina.Text.ReadOnly;
using PetRenamer.PetNicknames.Hooking.Structs;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class PartyHook : HookableElement
{
    private bool hasRegisteredListener = false;

    private ulong[] partyGroup = new ulong[8]; // a party is always 8 in size

    public PartyHook(DalamudServices services, IPetServices petServices, IPettableUserList userList, IPettableDirtyListener dirtyListener) 
        : base(services, userList, petServices, dirtyListener) { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup,           "_PartyList", LifeCycleUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_PartyList", LifeCycleUpdate);
    }

    public override void Refresh()
    {
        if (hasRegisteredListener)
        {
            return;
        }

        hasRegisteredListener = true;

        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostDraw, "_PartyList", LifeCycleUpdateRefresh);
    }

    private bool CanContinue(AtkUnitBase* baseD) 
        => baseD != null && baseD->IsFullyLoaded() && baseD->IsVisible;

    private void LifeCycleUpdate(AddonEvent aEvent, AddonArgs args) 
        => Update((AtkUnitBase*)args.Addon.Address);

    private void LifeCycleUpdateRefresh(AddonEvent aEvent, AddonArgs args)
    {
        Update((AtkUnitBase*)args.Addon.Address);

        hasRegisteredListener = false;
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdateRefresh);
    }

    private void Update(AtkUnitBase* baseD)
    {
        if (!CanContinue(baseD))
        {
            return;
        }

        SetPetName  ((PetNicknamesAddonPartyList*)baseD);
        SetCastlist ((AddonPartyList*)baseD);
    }

    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdateRefresh);
    }

    private void SetPetName(PetNicknamesAddonPartyList* partyNode)
    {
        IPettableUser? localPlayer = UserList.LocalPlayer;

        if (localPlayer == null)
        {
            return;
        }

        IPettablePet? pet = localPlayer.GetYoungestPet(IPettableUser.PetFilter.BattlePet);

        if (pet == null)
        {
            return;
        }
        
        PetServices.StringHelper.ReplaceATKString(PetServices.Configuration.ShowOnPartyListColour, partyNode->Pet.Name, pet, NameType.Raw);
    }

    private void SetCastlist(AddonPartyList* partyNode)
    {
        if (!PetServices.Configuration.ShowOnCastbarsColour.Enabled)
        {
            return;
        }

        SetupPartyList();

        int index = -1;

        foreach (PartyListMemberStruct member in partyNode->PartyMembers)
        {
            index++;

            if (index < 0 || index > partyGroup.Length)
            {
                continue;
            }

            if (member.Name == null)
            {
                continue;
            }

            if (member.CastingProgressBar == null)
            {
                continue;
            }

            if (!member.CastingProgressBar->AtkResNode.IsVisible())
            {
                continue;
            }

            string castString = new ReadOnlySeStringSpan(member.CastingActionName->NodeText).ExtractText();

            if (castString == string.Empty)
            {
                continue;
            }

            ulong contentId = partyGroup[index];

            if (contentId == 0) // this means there is no party active
            {
                IPettableUser? localUser = UserList.LocalPlayer;
                
                if (localUser == null)
                {
                    return;
                }

                contentId = localUser.ContentID;
            }

            IPettableUser? user = UserList.GetUserFromContentId(contentId);

            if (user == null)
            {
                continue;
            }

            IPetSheetData? data = PetServices.PetSheets.GetPetFromAction(user.CurrentCastID, in user);

            if (data == null)
            {
                continue;
            }

            PetServices.StringHelper.ReplaceATKString(PetServices.Configuration.ShowOnCastbarsColour, member.CastingActionName, data, NameType.Action, user);
        }
    }

    private void SetupPartyList()
    {
        GroupManager* gManager = (GroupManager*)DalamudServices.PartyList.GroupManagerAddress;

        if (gManager == null) 
        { 
            return; 
        }

        bool isCrossParty = IsCrossParty();

        partyGroup = new ulong[8];

        // We can assume partyGroup is 8 in length, and so is the struct with party members, Thanks SE!
        for (int i = 0; i < partyGroup.Length; i++)
        {
            PartyMember member = gManager->MainGroup.PartyMembers[i];

            ulong contentId = member.ContentId;

            int? index;

            if (isCrossParty)
            {
                index = GetCrossPartyIndex(contentId);
            }
            else
            {
                index = GetNormalPartyIndex(contentId);
            }

            if (index == null)
            {
                continue;
            }

            if (index < 0 || index >= partyGroup.Length)
            {
                continue;
            }

            partyGroup[index.Value] = contentId;
        }
    }

    private bool IsCrossParty()
    {
        bool isCrossRealm     = InfoProxyCrossRealm.Instance()->IsCrossRealm;
        bool noMembersInGroup = GroupManager.Instance()->MainGroup.MemberCount < 1;

        return (isCrossRealm && noMembersInGroup);
    }

    private int? GetCrossPartyIndex(ulong contentId)
    {
        if (InfoProxyCrossRealm.Instance() == null)
        {
            return null;
        }

        CrossRealmMember* member = InfoProxyCrossRealm.GetMemberByContentId(contentId);

        if (member == null)
        {
            return null;
        }

        return member->MemberIndex;
    }

    private int? GetNormalPartyIndex(ulong contentId)
    {
        int   memberCount    = GroupManager.Instance()->MainGroup.MemberCount;
        bool  foundSelf      = false;
        ulong localContentId = UserList.LocalPlayer?.ContentID ?? 0;

        for (int i = 0; i < memberCount; i++)
        {
            int actualCurrent = i;

            if (!foundSelf)
            {
                actualCurrent++;
            }

            PartyMember* member = GroupManager.Instance()->MainGroup.GetPartyMemberByIndex(i);

            if (member == null)
            {
                continue;
            }

            if (member->ContentId == localContentId)
            {
                foundSelf     = true;
                actualCurrent = 0;
            }

            if (contentId == member->ContentId)
            {
                return actualCurrent;
            }
        }

        return null;
    }
}
