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

        MountContainer mount = playerCharacter->Mount;

        CompanionData? data = null;
        MountData? mountData = null;

        if(playerCompanion != null)
            data = new CompanionData(playerCompanion, playerCompanion->Character.CharacterData.ModelSkeletonId);

        if (mount.MountObject != null)
            mountData = new MountData(mount.MountObject, playerCharacter->Mount.MountId, playerCharacter->Mount.Flags);

        return new PlayerData(me, me->Gender, playerCharacter->HomeWorld, PluginHandlers.ClientState.LocalPlayer?.Customize[(int)Dalamud.Game.ClientState.Objects.Enums.CustomizeIndex.Race] ?? -1, PluginHandlers.ClientState.LocalPlayer?.Customize[(int)Dalamud.Game.ClientState.Objects.Enums.CustomizeIndex.Tribe] ?? -1, data, mountData);
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
    internal MountData? mountData;

    unsafe public PlayerData(GameObject* playerGameObject, byte gender, ushort homeWorld, int race, int tribe, CompanionData? companionData, MountData? mountData)
    {
        this.playerGameObject = playerGameObject;
        this.namePtr = playerGameObject->Name;
        this.gender = gender;
        this.homeWorld = homeWorld;
        this.companionData = companionData;
        this.mountData = mountData;
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

internal struct MountData
{
    unsafe internal Character* mount;
    internal ushort mountID;
    internal byte flags;

    unsafe public MountData(Character* mount, ushort mountID, byte flags)
    {
        this.mount = mount;
        this.mountID = mountID;
        this.flags = flags;
    }
}
