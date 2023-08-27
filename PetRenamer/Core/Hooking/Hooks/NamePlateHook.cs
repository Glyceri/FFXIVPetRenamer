using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Utilization.Enum;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Core.Hooking.Hooks;

//[Hook]
public unsafe sealed class NamePlateHook : HookableElement
{
    [Signature("40 55 56 57 41 56 48 81 EC ?? ?? ?? ?? 48 8B 84 24", DetourName = nameof(UpdateNameplateDetour))]
    private Hook<UpdateNameplateDelegate>? nameplateHook = null;

    [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 4C 89 44 24 ?? 57 41 54 41 55 41 56 41 57 48 83 EC 20 48 8B 74 24 ??", DetourName = nameof(UpdateNameplateNpcDetour))]
    private Hook<UpdateNameplateNpcDelegate>? nameplateHookMinion;

    private delegate void* UpdateNameplateDelegate(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* battleChara, int numArrayIndex, int stringArrayIndex);
    private delegate void* UpdateNameplateNpcDelegate(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* gameObject, int numArrayIndex, int stringArrayIndex);

    public void* UpdateNameplateDetour(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Character.BattleChara* battleChara, int numArrayIndex, int stringArrayIndex)
    {
        if (!PluginLink.Configuration.displayCustomNames) return nameplateHook!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, battleChara, numArrayIndex, stringArrayIndex);
        SerializableNickname nickname = NicknameUtils.instance.GetFromGameObjectPtr(&battleChara->Character.GameObject, PetType.BattlePet);
        if (nickname?.Valid() ?? false)
            namePlateInfo->Name.SetString(nickname.Name);
        return nameplateHook!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, battleChara, numArrayIndex, stringArrayIndex);
    }

    public void* UpdateNameplateNpcDetour(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* gameObject, int numArrayIndex, int stringArrayIndex)
    {
        if (!PluginLink.Configuration.displayCustomNames) return nameplateHookMinion!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, gameObject, numArrayIndex, stringArrayIndex);
        SerializableNickname nickname = NicknameUtils.instance.GetFromGameObjectPtr(gameObject, PetType.Companion);
        if(nickname?.Valid() ?? false)
            namePlateInfo->Name.SetString(nickname.Name);
        return nameplateHookMinion!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, gameObject, numArrayIndex, stringArrayIndex);
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
