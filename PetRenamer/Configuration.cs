using Dalamud.Configuration;
using Dalamud.Plugin;
using PetRenamer.Core.Serialization;
using System;

namespace PetRenamer
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public SerializableNickname[]? nicknames = null;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;
        [NonSerialized]
        private PetRenamerPlugin plugin;

        public void Initialize(DalamudPluginInterface pluginInterface, PetRenamerPlugin plugin)
        {
            this.PluginInterface = pluginInterface;
            this.plugin = plugin;

            if(nicknames == null) nicknames = new SerializableNickname[0];
        }

        public void Save()
        {
            if (PluginInterface == null) return;
            this.PluginInterface?.SavePluginConfig(this);
        }
    }
}
