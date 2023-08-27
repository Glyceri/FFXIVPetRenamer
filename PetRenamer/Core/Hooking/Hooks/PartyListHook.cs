using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.PettableUserSystem;
using System;
using System.Runtime.InteropServices;

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

        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return addonupdatehook!.Original(baseD);
        if (!user.HasBattlePet) return addonupdatehook!.Original(baseD);
        if (user.BattlePetCustomName != string.Empty)
            partyNode->Pet.Name->SetText(user.BattlePetCustomName);
        return addonupdatehook!.Original(baseD);
    }

    internal override void OnDispose()
    {
        addonupdatehook?.Disable();
        addonupdatehook?.Dispose();
    }
}
