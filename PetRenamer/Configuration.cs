using Dalamud.Configuration;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using System;

namespace PetRenamer
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public SerializableNickname[]? nicknames = null;

        public bool displayCustomNames = true;

        public void Initialize()
        {
            if(nicknames == null) nicknames = new SerializableNickname[0];
        }

        public void ClearNicknames()
        {
            nicknames = new SerializableNickname[0];
            Save();
            PluginLink.WindowHandler.CloseAllWindows();
        }

        public void Save()
        {
            PluginLink.DalamudPlugin.SavePluginConfig(this);
            Globals.RedrawPet = true;
        }
    }
}
