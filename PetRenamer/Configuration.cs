using Dalamud.Configuration;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using System;

namespace PetRenamer;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 2;

    public SerializableNickname[]? users = null;

    public bool displayCustomNames = true;

    public void Initialize()
    {
        if(users == null) users = new SerializableNickname[0];
    }

    public void ClearNicknames()
    {
        PluginLink.WindowHandler.CloseAllWindows();
        users = new SerializableNickname[0];
        Save();
    }

    public void Save()
    {
        PluginLink.DalamudPlugin.SavePluginConfig(this);
        Globals.RedrawPet = true;
    }
}
