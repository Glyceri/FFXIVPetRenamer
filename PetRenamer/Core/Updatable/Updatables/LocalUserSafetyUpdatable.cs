﻿using Dalamud.Game;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Serialization;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class LocalUserSafetyUpdatable : Updatable
{
    PlayerUtils playerUtils { get; set; } = null!;
    ConfigurationUtils configurationUtils { get; set; } = null!;

    public LocalUserSafetyUpdatable() : base()
    {
        playerUtils = PluginLink.Utils.Get<PlayerUtils>();
        configurationUtils = PluginLink.Utils.Get<ConfigurationUtils>();
    }

    public override void Update(Framework frameWork)
    {
        if (configurationUtils.GetLocalUser() != null) return;
        if (!playerUtils.PlayerDataAvailable()) return;


        PlayerData? playerData = playerUtils.GetPlayerData();
        if(playerData == null) return;
        SerializableUser localUser = new SerializableUser(playerData.Value.playerName, playerData.Value.homeWorld);
        configurationUtils.AddNewUser(localUser);
    }
}

