using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class NamePlateHook : HookableElement
{
    public delegate void* UpdateNameplateDelegate(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, BattleChara* battleChara, int numArrayIndex, int stringArrayIndex);
    public delegate void* UpdateNameplateNpcDelegate(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, GameObject* gameObject, int numArrayIndex, int stringArrayIndex);

    [Signature("40 56 57 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B 84 24 ?? ?? ?? ??", DetourName = nameof(UpdateNameplateDetour))]
    readonly Hook<UpdateNameplateDelegate>? nameplateHook = null;

    [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 4C 89 44 24 ?? 57 41 54 41 55 41 56 41 57 48 83 EC 20 48 8B 74 24 ??", DetourName = nameof(UpdateNameplateNpcDetour))]
    readonly Hook<UpdateNameplateNpcDelegate>? nameplateHookMinion = null;

    IPettableUserList PettableUserList { get; init; }
    IPetLog PetLog { get; init; }

    public NamePlateHook(DalamudServices services, IPettableUserList pettableUserList, IPetLog petLog) : base(services) 
    {
        PettableUserList = pettableUserList;
        PetLog = petLog;
    }

    public override void Init()
    {
        nameplateHook?.Enable();
        nameplateHookMinion?.Enable();
    }

    public override void Dispose()
    {
        nameplateHook?.Dispose();
        nameplateHookMinion?.Dispose();
    }

    public void* UpdateNameplateDetour(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* battleChara, int numArrayIndex, int stringArrayIndex)
    {
        SetNameplate(namePlateInfo, (nint)battleChara);
        return nameplateHook!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, battleChara, numArrayIndex, stringArrayIndex);
    }

    public void* UpdateNameplateNpcDetour(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* gameObject, int numArrayIndex, int stringArrayIndex)
    {
        SetNameplate(namePlateInfo, (nint)gameObject);
        return nameplateHookMinion!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, gameObject, numArrayIndex, stringArrayIndex);
    }

    void SetNameplate(RaptureAtkModule.NamePlateInfo* namePlateInfo, nint obj)
    {
        IPettablePet? pPet = PettableUserList.GetPet(obj);
        if (pPet == null) return;
        string? customPetName = pPet.CustomName;
        if (customPetName == null) return;

        namePlateInfo->Name.SetString(customPetName);
        namePlateInfo->IsDirty = true;
    }
}
