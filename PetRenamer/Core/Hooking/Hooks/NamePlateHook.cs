using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Ipc.PenumbraIPCHelper;
using Dalamud.Logging;

namespace PetRenamer.Core.Hooking.Hooks;

// Signatures from: https://github.com/Caraxi/Honorific/blob/master/Plugin.cs
// I store these so when they inevitably change, I can just yoink them again from there.

[Hook]
public unsafe sealed class NamePlateHook : HookableElement
{
    [Signature("40 55 56 57 41 56 48 81 EC ?? ?? ?? ?? 48 8B 84 24", DetourName = nameof(UpdateNameplateDetour))]
    Hook<Delegates.UpdateNameplateDelegate>? nameplateHook = null;

    [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 4C 89 44 24 ?? 57 41 54 41 55 41 56 41 57 48 83 EC 20 48 8B 74 24 ??", DetourName = nameof(UpdateNameplateNpcDetour))]
    Hook<Delegates.UpdateNameplateNpcDelegate>? nameplateHookMinion;

    public void* UpdateNameplateDetour(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* battleChara, int numArrayIndex, int stringArrayIndex)
    {
        if (PluginLink.Configuration.displayCustomNames) SetNameplate(namePlateInfo, (nint)battleChara);
        return nameplateHook!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, battleChara, numArrayIndex, stringArrayIndex);
    }

    public void* UpdateNameplateNpcDetour(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* gameObject, int numArrayIndex, int stringArrayIndex)
    {
        if (PluginLink.Configuration.displayCustomNames) SetNameplate(namePlateInfo, (nint)gameObject);
        return nameplateHookMinion!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, gameObject, numArrayIndex, stringArrayIndex);
    }

    string lastBattlePetNameToUse = string.Empty;

    void SetNameplate(RaptureAtkModule.NamePlateInfo* namePlateInfo, nint obj)
    {
        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
        {
            if (!user.UserExists) continue;
            string nameToUse = string.Empty;
            if (user.nintCompanion == obj) nameToUse = user.CustomCompanionName;
            if (user.nintBattlePet == obj) nameToUse = user.BattlePetCustomName;
            if (nameToUse != string.Empty) 
            { 
                namePlateInfo->Name.SetString(nameToUse);
                break; 
            }
        }
    }

    internal override void OnInit()
    {
        nameplateHook?.Enable();
        nameplateHookMinion?.Enable();
    }

    internal override void OnDispose()
    {
        nameplateHook?.Disable();
        nameplateHook?.Dispose();
        nameplateHook = null;

        nameplateHookMinion?.Disable();
        nameplateHookMinion?.Dispose();
        nameplateHookMinion = null;
    }
}
