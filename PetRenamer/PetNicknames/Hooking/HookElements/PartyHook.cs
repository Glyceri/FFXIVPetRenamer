using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.NativeWrapper;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
using FFXIVClientStructs.FFXIV.Component.GUI;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonPartyList;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using Lumina.Text.ReadOnly;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class PartyHook : HookableElement
{
    public PartyHook(DalamudServices services, IPetServices petServices) 
        : base(services, petServices) { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup,           "_PartyList", LifeCycleUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_PartyList", LifeCycleUpdate);
    }
    
    protected override void OnDispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
    }

    public override void Refresh()
    {
        RefreshAddon("_PartyList");
    }
    
    private bool CanContinue(AtkUnitBase* baseD) 
        => baseD != null && baseD->IsFullyLoaded() && baseD->IsVisible;

    private void LifeCycleUpdate(AddonEvent aEvent, AddonArgs args) 
        => Update((AtkUnitBase*)args.Addon.Address);

    private void Update(AtkUnitBase* baseD)
    {
        if (!CanContinue(baseD))
        {
            return;
        }

        SetPetName  ((AddonPartyList*)baseD);
        SetCastList ((AddonPartyList*)baseD);
    }

    private PartyListMemberStruct GetPet(AddonPartyList* partyList)
    {
        bool usePetSlot = PartyListNumberArray.Instance()->UsePetSlot;
        
        return (usePetSlot ? partyList->Pet : partyList->SpecialPet);
    }
    
    private void SetPetName(AddonPartyList* partyNode)
    {
        IPettableUser? localPlayer = PetServices.UserList.LocalPlayer;

        if (localPlayer == null)
        {
            return;
        }

        IPettablePet? pet = localPlayer.GetYoungestPet(SkeletonType.BattlePet);

        if (pet == null)
        {
            return;
        }
        
        PartyListMemberStruct petMember = GetPet(partyNode);
        
        PetServices.StringHelper.ReplaceAtkString(PetServices.Configuration.ShowOnPartyListColour, petMember.Name, pet, NameType.Raw);
    }

    private void SetCastList(AddonPartyList* partyNode)
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

            member.CastingActionName->TextFlags &= ~(TextFlags.Ellipsis | TextFlags.OverflowHidden);
            member.CastingActionName->SetWidth(197);
            member.CastingActionName->X = 0;
            member.CastingActionName->IsDirty = true;
            
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

            IPetSheetData? data = PetServices.PetSheets.GetPetFromAction(user.CurrentCastId);

            if (data == null)
            {
                continue;
            }
            
            data = PetServices.PetSheets.MakeSoft(user, data);

            bool replaced = PetServices.StringHelper.ReplaceAtkString(PetServices.Configuration.ShowOnCastbarsColour, member.CastingActionName, data, NameType.Action, user);
            
            if (!PetServices.Configuration.allowPartySummonCutoff)
            {
                continue;
            }
            
            if (!replaced)
            {
                continue;
            }
            
            ushort baseWidth  = 0;
            ushort baseHeight = 0;
            
            member.CastingActionName->GetTextDrawSize(&baseWidth, &baseHeight);
            
            ushort textboxWidth = member.CastingActionName->GetWidth();
            
            if (baseWidth < textboxWidth)
            {
                return;
            }
            
            member.CastingActionName->TextFlags |= (TextFlags.Ellipsis | TextFlags.OverflowHidden);
            member.CastingActionName->SetWidth(197 - 22);
            member.CastingActionName->X = 22;
        }
    }
}
