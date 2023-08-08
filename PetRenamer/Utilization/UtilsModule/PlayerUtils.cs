using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using FFCompanion = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class PlayerUtils : UtilsRegistryType, ISingletonBase<PlayerUtils>
{
    public static PlayerUtils instance { get; set; } = null!;

    public bool PlayerDataAvailable() => PluginHandlers.ClientState.LocalPlayer != null;

    unsafe internal PlayerData? GetPlayerData(bool hasPetOut = false)
    {
        GameObject* me = GameObjectManager.GetGameObjectByIndex(0);
        if (me == null) return null;
        if (!me->IsCharacter()) return null;
        Character* playerCharacter = (Character*)me;
        if (playerCharacter == null) return null;
        FFCompanion* meCompanion = (FFCompanion*)me;
        if (meCompanion == null) return null;

        FFCompanion* playerCompanion = meCompanion->Character.Companion.CompanionObject;

        CompanionData? data = null;

        if(playerCompanion != null)
            data = new CompanionData(playerCompanion, playerCompanion->Character.CharacterData.ModelSkeletonId);

        int petType = -1;
        if (hasPetOut) petType = RemapUtils.instance.GetPetIDFromClass(playerCharacter->CharacterData.ClassJob);

        return new PlayerData(me, me->Gender, playerCharacter->HomeWorld, PluginHandlers.ClientState.LocalPlayer?.Customize[(int)Dalamud.Game.ClientState.Objects.Enums.CustomizeIndex.Race] ?? -1, PluginHandlers.ClientState.LocalPlayer?.Customize[(int)Dalamud.Game.ClientState.Objects.Enums.CustomizeIndex.Tribe] ?? -1, petType, playerCharacter->CharacterData.ClassJob, data);
    }
}


internal struct PlayerData
{
    internal unsafe GameObject* playerGameObject;
    unsafe internal string playerName => StringUtils.instance.FromBytes(StringUtils.instance.GetBytes(namePtr));
    unsafe internal byte* namePtr;
    internal ushort homeWorld;
    internal byte gender;
    internal int race;
    internal int tribe;
    internal int job;
    internal int battlePetID;

    internal CompanionData? companionData;

    unsafe public PlayerData(GameObject* playerGameObject, byte gender, ushort homeWorld, int race, int tribe, int battlePetID, int job, CompanionData? companionData)
    {
        this.playerGameObject = playerGameObject;
        this.namePtr = playerGameObject->Name;
        this.gender = gender;
        this.homeWorld = homeWorld;
        this.companionData = companionData;
        this.battlePetID = battlePetID;
        this.race = race;
        this.tribe = tribe;
        this.job = job;
    }
}

internal struct CompanionData
{
    unsafe internal FFCompanion* companion;
    unsafe internal string currentCompanionName => StringUtils.instance.FromBytes(StringUtils.instance.GetBytes(namePtr));
    unsafe internal byte* namePtr;
    internal int currentModelID;

    unsafe public CompanionData(FFCompanion* companion, int currentModelID)
    {
        this.namePtr = companion->Character.GameObject.Name;
        this.companion = companion;
        this.currentModelID = currentModelID;
    }
}