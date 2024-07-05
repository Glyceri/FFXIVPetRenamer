using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonPartyList;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class PartyHook : HookableElement
{
    public PartyHook(DalamudServices services, IPetServices petServices, IPettableUserList userList) : base(services, userList, petServices) { }

    public override void Init()
    {
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "_PartyList", LifeCycleUpdate);
        DalamudServices.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "_PartyList", LifeCycleUpdate);
    }

    bool CanContinue(AtkUnitBase* baseD) => !(!baseD->IsVisible || !true || baseD == null);

    void LifeCycleUpdate(AddonEvent aEvent, AddonArgs args) => Update((AtkUnitBase*)args.Addon);

    void Update(AtkUnitBase* baseD)
    {
        if (!CanContinue(baseD)) return;
        SetPetname((AddonPartyList*)baseD);
        SetCastlist((AddonPartyList*)baseD);
    }

    public override void Dispose()
    {
        DalamudServices.AddonLifecycle.UnregisterListener(LifeCycleUpdate);
    }

    void SetPetname(AddonPartyList* partyNode)
    {
        IPettableUser? localPlayer = UserList.LocalPlayer;
        if (localPlayer == null) return;

        IPettablePet? pet = localPlayer.GetYoungestPet(IPettableUser.PetFilter.BattlePet);
        if (pet == null) return;

        string? lastPetname = pet.CustomName;
        if (lastPetname == string.Empty || lastPetname == null) return;
        partyNode->Pet.Name->SetText(lastPetname);
    }

    void SetCastlist(AddonPartyList* partyNode)
    {
        foreach (PartyListMemberStruct member in partyNode->PartyMembers)
        {
            if (member.Name == null) continue;
            if (member.CastingProgressBar == null) continue;
            if (!member.CastingProgressBar->AtkResNode.IsVisible()) continue;

            string memberName = member.Name->NodeText.ToString();
            if (memberName == string.Empty) continue;

            string[] splitName = memberName.Split(' ');
            if (splitName.Length < 2) continue;

            memberName = $"{splitName[splitName.Length - 2]} {splitName[splitName.Length - 1]}";

            string castString = member.CastingActionName->NodeText.ToString();
            if (castString == string.Empty) continue;

            IPettableUser? user = UserList.GetUser(memberName);
            if (user == null) { PetServices.PetLog.Log("User not found: " + memberName); continue; }

            IPetSheetData? data = PetServices.PetSheets.GetPetFromString(castString, ref user, true);
            if (data == null) continue;

            string? customName = user.DataBaseEntry.GetName(data.Model);
            if (customName == null) continue;

            PetServices.StringHelper.ReplaceATKString(member.CastingActionName, castString, customName, data, false);
        }
    }
}
