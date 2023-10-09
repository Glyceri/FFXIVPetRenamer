using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System.Collections.Generic;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonPartyList;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal unsafe class PartyListHook : HookableElement
{
    // VVVVVV ACTUAL BYTE CODE GENEROUSLY PROVIDED BY: Nuko
    // [Signature("48 83 EC ?? F6 81 ?? ?? ?? ?? ?? 0F 84 ?? ?? ?? ?? 8B 81", DetourName = nameof(PartyListHookUpdate))]

    internal override void OnInit()
    {
        PluginHandlers.AddonLifecycle.RegisterListener(AddonEvent.PreUpdate, "_PartyList", LifeCycleUpdate);
    }

    void LifeCycleUpdate(AddonEvent aEvent, AddonArgs args) => Update((AtkUnitBase*)args.Addon);

    void Update(AtkUnitBase* baseD)
    {
        if (!CanContinue(baseD)) return;
        SetPetname((AddonPartyList*)baseD);
        SetCastlist((AddonPartyList*)baseD);
    }

    bool CanContinue(AtkUnitBase* baseD) => !(!baseD->IsVisible || !PluginLink.Configuration.displayCustomNames || baseD == null);

    void SetPetname(AddonPartyList* partyNode)
    {
        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        if (!user.BattlePet.Has) return;
        if (user.BattlePet.CustomName == string.Empty) return;
        partyNode->Pet.Name->SetText(user.BattlePet.CustomName);
    }

    void SetCastlist(AddonPartyList* partyNode)
    {
        List<PartyListMemberStruct> partyMemberNames = new List<PartyListMemberStruct>() {
            partyNode->PartyMember.PartyMember0,
            partyNode->PartyMember.PartyMember1,
            partyNode->PartyMember.PartyMember2,
            partyNode->PartyMember.PartyMember3,
            partyNode->PartyMember.PartyMember4,
            partyNode->PartyMember.PartyMember5,
            partyNode->PartyMember.PartyMember6,
            partyNode->PartyMember.PartyMember7
        };

        TooltipHelper.partyListInfos.Clear();

        foreach (PartyListMemberStruct member in partyMemberNames)
        {
            if (member.Name == null) continue;
            if (member.CastingProgressBar == null) continue;
            if (!member.CastingProgressBar->AtkResNode.IsVisible) continue;

            string memberName = member.Name->NodeText.ToString() ?? string.Empty;
            if (memberName == string.Empty) continue;
            string[] splitName = memberName.Split(' ');
            if (splitName.Length != 3) continue;
            memberName = $"{splitName[1]} {splitName[2]}";

            if (!PluginLink.Configuration.allowCastBarPet || !PluginLink.Configuration.displayCustomNames) continue;

            string castString = member.CastingActionName->NodeText.ToString();
            if (castString == string.Empty) continue;

            PettableUser? user = PluginLink.PettableUserHandler.GetUser(memberName);
            if (user == null) continue;

            (int, string) pet = PettableUserUtils.instance.GetNameRework(castString, ref user, true);
            StringUtils.instance.ReplaceAtkString(member.CastingActionName, pet.Item2, user.SerializableUser.GetNameFor(pet.Item1));
        }
    }
}
