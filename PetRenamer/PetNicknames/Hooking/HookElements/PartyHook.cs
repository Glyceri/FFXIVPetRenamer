using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
using FFXIVClientStructs.FFXIV.Component.GUI;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonPartyList;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using Lumina.Text.ReadOnly;
using PetRenamer.PetNicknames.Hooking.Structs;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class PartyHook : HookableElement
{
    private bool hasRegisteredListener = false;
    
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

        IPettablePet? pet = localPlayer.GetYoungestPet(SkeletonType.BattlePet);

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

        int index = -1;

        foreach (PartyListMemberStruct member in partyNode->PartyMembers)
        {
            index++;

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
            
            IPettableUser? user = PetServices.Party[index];

            if (user == null)
            {
                continue;
            }

            IPetSheetData? data = PetServices.PetSheets.GetPetFromAction(user.CurrentCastId, in user);

            if (data == null)
            {
                continue;
            }

            PetServices.StringHelper.ReplaceATKString(PetServices.Configuration.ShowOnCastbarsColour, member.CastingActionName, data, NameType.Action, user);
        }
    }
}
