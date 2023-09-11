using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonPartyList;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
internal unsafe class PartyListHook : HookableElement
{
    // VVVVVV ACTUAL BYTE CODE GENEROUSLY PROVIDED BY: Nuko
    // [Signature("48 83 EC ?? F6 81 ?? ?? ?? ?? ?? 0F 84 ?? ?? ?? ?? 8B 81", DetourName = nameof(PartyListHookUpdate))]

    private Hook<Delegates.AddonUpdate>? addonupdatehook = null;

    AddonPartyList* partyList;


    internal override void OnUpdate(Framework framework)
    {
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        partyList = (AddonPartyList*)PluginHandlers.GameGui.GetAddonByName("_PartyList");
        addonupdatehook ??= Hook<Delegates.AddonUpdate>.FromAddress(new nint(partyList->AtkUnitBase.AtkEventListener.vfunc[PluginConstants.AtkUnitBaseUpdateIndex]), Update);
        addonupdatehook?.Enable();
    }

    byte Update(AtkUnitBase* baseD)
    {
        if (!PluginLink.Configuration.displayCustomNames) return addonupdatehook!.Original(baseD);
        string? name = Marshal.PtrToStringUTF8((IntPtr)baseD->Name);
        if (!baseD->IsVisible || name is not "_PartyList")
            return addonupdatehook!.Original(baseD);
        AddonPartyList* partyNode = (AddonPartyList*)baseD;
        if (partyNode == null) return addonupdatehook!.Original(baseD);
         SetPetname(baseD, partyNode);
        SetCastlist(baseD, partyNode);
        return addonupdatehook!.Original(baseD);
    }

    void SetPetname(AtkUnitBase* baseD, AddonPartyList* partyNode)
    {
        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        if (!user.HasBattlePet) return;
        if (user.BattlePetCustomName != string.Empty)
            partyNode->Pet.Name->SetText(user.BattlePetCustomName);
    }

    void SetCastlist(AtkUnitBase * baseD, AddonPartyList * partyNode)
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

        foreach(PartyListMemberStruct member in partyMemberNames)
        {
            string memberName = member.Name->NodeText.ToString()!;
            if (memberName == null) continue;
            if (memberName == string.Empty) continue;
            string[] splitName = memberName.Split(' ');
            if(splitName.Length != 3) continue;
            memberName = $"{splitName[1]} {splitName[2]}";

            BattleChara* bChara = PluginLink.CharacterManager->LookupBattleCharaByName(memberName);
            if (bChara == null) continue;
            BattleChara* carbuncle = PluginLink.CharacterManager->LookupPetByOwnerObject(bChara);
            BattleChara* chocobo = PluginLink.CharacterManager->LookupBuddyByOwnerObject(bChara);
            TooltipHelper.partyListInfos.Add(new PartyListInfo(memberName, carbuncle != null, chocobo != null));

            string castString = member.CastingActionName->NodeText.ToString();
            if (castString == string.Empty) continue;

            PettableUser? user = PluginLink.PettableUserHandler.GetUser(memberName);
            if (user == null) continue;

            (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(user, castString);
            if (PluginLink.Configuration.allowCastBarPet && PluginLink.Configuration.displayCustomNames)
                StringUtils.instance.ReplaceAtkString(member.CastingActionName, validNames);
        }
    }

    internal override void OnDispose()
    {
        addonupdatehook?.Disable();
        addonupdatehook?.Dispose();
    }
}
