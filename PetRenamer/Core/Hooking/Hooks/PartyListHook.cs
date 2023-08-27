using Dalamud.Configuration;
using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.Enum;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Runtime.InteropServices;

namespace PetRenamer.Core.Hooking.Hooks;

//[Hook]
internal unsafe class PartyListHook : HookableElement
{
    // VVVVVV ACTUAL BYTE CODE GENEROUSLY PROVIDED BY: Nuko
    // [Signature("48 83 EC ?? F6 81 ?? ?? ?? ?? ?? 0F 84 ?? ?? ?? ?? 8B 81", DetourName = nameof(PartyListHookUpdate))]

    private Hook<Delegates.AddonUpdate>? addonupdatehook = null;

    AddonPartyList* partyList;

    GameObject* pet;

    internal override void OnUpdate(Framework framework)
    {
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        pet = &PluginLink.CharacterManager->LookupPetByOwnerObject((BattleChara*)GameObjectManager.GetGameObjectByIndex(0))->Character.GameObject;
        partyList = (AddonPartyList*)PluginHandlers.GameGui.GetAddonByName("_PartyList");
        addonupdatehook ??= Hook<Delegates.AddonUpdate>.FromAddress(new nint(partyList->AtkUnitBase.AtkEventListener.vfunc[42]), Update);
        addonupdatehook?.Enable();
    }

    byte Update(AtkUnitBase* baseD)
    {
        if (!PluginLink.Configuration.displayCustomNames) return addonupdatehook!.Original(baseD);
        string? name = Marshal.PtrToStringUTF8((IntPtr)baseD->Name);
        if (!baseD->IsVisible || name is not "_PartyList")
            return addonupdatehook!.Original(baseD);
        AddonPartyList* partyNode = (AddonPartyList*)baseD;
        SerializableNickname nickname = NicknameUtils.instance.GetFromGameObjectPtr(pet, PetType.BattlePet);
        if(nickname?.Valid() ?? false)
            partyNode->Pet.Name->SetText(nickname.Name);
        return addonupdatehook!.Original(baseD);
    }

    internal override void OnDispose()
    {
        addonupdatehook?.Disable();
        addonupdatehook?.Dispose();
    }
}
