using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.Attributes;

using FFCompanion = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class PlayerUtils : UtilsRegistryType
{
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

        return new PlayerData(me, me->Gender, playerCharacter->HomeWorld, data);
    }
}


internal struct PlayerData
{
    StringUtils stringUtils => PluginLink.Utils.Get<StringUtils>();
    unsafe GameObject* playerGameObject;
    unsafe internal string playerName => stringUtils.FromBytes(stringUtils.GetBytes(playerGameObject->Name));
    internal ushort homeWorld;
    internal byte gender;

    internal CompanionData? companionData;

    unsafe public PlayerData(GameObject* playerGameObject, byte gender, ushort homeWorld, CompanionData? companionData)
    {
        this.playerGameObject = playerGameObject;
        this.gender = gender;
        this.homeWorld = homeWorld;
        this.companionData = companionData;
    }
}

internal struct CompanionData
{
    StringUtils stringUtils => PluginLink.Utils.Get<StringUtils>();
    unsafe internal FFCompanion* companion;
    unsafe internal string currentCompanionName => stringUtils.FromBytes(stringUtils.GetBytes(companion->Character.GameObject.Name));
    internal int currentModelID;

    unsafe public CompanionData(FFCompanion* companion, int currentModelID)
    {
        this.companion = companion;
        this.currentModelID = currentModelID;
    }
}
