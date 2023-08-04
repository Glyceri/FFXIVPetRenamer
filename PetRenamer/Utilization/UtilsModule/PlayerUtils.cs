using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.Attributes;
using static FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using FFCompanion = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class PlayerUtils : UtilsRegistryType
{
    public bool PlayerDataAvailable() => PluginHandlers.ClientState.LocalPlayer != null;

    unsafe internal PlayerData? GetPlayerData()
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

        return new PlayerData(me, me->Gender, playerCharacter->HomeWorld, PluginHandlers.ClientState.LocalPlayer?.Customize[(int)Dalamud.Game.ClientState.Objects.Enums.CustomizeIndex.Race] ?? -1, PluginHandlers.ClientState.LocalPlayer?.Customize[(int)Dalamud.Game.ClientState.Objects.Enums.CustomizeIndex.Tribe] ?? -1, data);
    }
}


internal struct PlayerData
{
    StringUtils stringUtils => PluginLink.Utils.Get<StringUtils>();
    unsafe GameObject* playerGameObject;
    unsafe internal string playerName => stringUtils.FromBytes(stringUtils.GetBytes(namePtr));
    unsafe internal byte* namePtr;
    internal ushort homeWorld;
    internal byte gender;
    internal int race;
    internal int tribe;

    internal CompanionData? companionData;

    unsafe public PlayerData(GameObject* playerGameObject, byte gender, ushort homeWorld, int race, int tribe, CompanionData? companionData)
    {
        this.playerGameObject = playerGameObject;
        this.namePtr = playerGameObject->Name;
        this.gender = gender;
        this.homeWorld = homeWorld;
        this.companionData = companionData;
        this.race = race;
        this.tribe = tribe;
    }
}

internal struct CompanionData
{
    StringUtils stringUtils => PluginLink.Utils.Get<StringUtils>();
    unsafe internal FFCompanion* companion;
    unsafe internal string currentCompanionName => stringUtils.FromBytes(stringUtils.GetBytes(namePtr));
    unsafe internal byte* namePtr;
    internal int currentModelID;

    unsafe public CompanionData(FFCompanion* companion, int currentModelID)
    {
        this.namePtr = companion->Character.GameObject.Name;
        this.companion = companion;
        this.currentModelID = currentModelID;
    }
}