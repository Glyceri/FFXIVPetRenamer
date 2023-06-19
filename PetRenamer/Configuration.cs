using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace PetRenamer
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public string CustomPetName { get; set; } = string.Empty;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;
        [NonSerialized]
        private PetRenamerPlugin plugin;

        public void Initialize(DalamudPluginInterface pluginInterface, PetRenamerPlugin plugin)
        {
            this.PluginInterface = pluginInterface;
            this.plugin = plugin;

            int counter = 0;

            foreach (char c in CustomPetName)
            {
                plugin.petName[counter] = (byte)c;
                counter++;
                if (counter == 64) break;
            }
        }

        public void Save()
        {
            if (PluginInterface == null) return;
            this.PluginInterface?.SavePluginConfig(this);
        }
    }
}
