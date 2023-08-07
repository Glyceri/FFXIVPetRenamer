using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Hooking.Attributes;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Updatable.Updatables;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.Hooking.Hooks;

[Hook]
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
        if (!PluginLink.Configuration.useNewNameSystem) return nameplateHook!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, battleChara, numArrayIndex, stringArrayIndex);
        foreach (FoundPlayerCharacter character in PluginLink.IpcStorage.characters) 
        {
            if (!character.HasBattlePet()) continue;
            if (!RemapUtils.instance.battlePetRemap.ContainsKey(battleChara->Character.CharacterData.ModelCharaId)) return nameplateHook!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, battleChara, numArrayIndex, stringArrayIndex);
            if (character.GetBattlePetObjectID() == battleChara->Character.GameObject.ObjectID)
            {
                SerializableNickname? nickname = NicknameUtils.instance.GetNicknameV2(character.associatedUser!, character.GetBattlePetID());
                if (nickname == null) continue;
                if (nickname.Name == string.Empty) continue;
                namePlateInfo->Name.SetString(nickname.Name);
            }
        }
        return nameplateHook!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, battleChara, numArrayIndex, stringArrayIndex);
    }

    public void* UpdateNameplateNpcDetour(RaptureAtkModule* raptureAtkModule, RaptureAtkModule.NamePlateInfo* namePlateInfo, NumberArrayData* numArray, StringArrayData* stringArray, FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* gameObject, int numArrayIndex, int stringArrayIndex)
    {
        if (!PluginLink.Configuration.useNewNameSystem) return nameplateHookMinion!.Original(raptureAtkModule, namePlateInfo, numArray, stringArray, gameObject, numArrayIndex, stringArrayIndex);
        foreach (FoundPlayerCharacter character in PluginLink.IpcStorage.characters)
        {
            if (!character.HasCompanion()) continue;
            if (character.GetCompanionID() == ((FFCharacter*)gameObject)->CharacterData.ModelSkeletonId)
            {
                SerializableNickname? nickname = NicknameUtils.instance.GetNicknameV2(character.associatedUser!, character.GetCompanionID());
                if (nickname == null) continue;
                if (nickname.Name == string.Empty) continue;
                namePlateInfo->Name.SetString(nickname.Name);
            }
        }
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
