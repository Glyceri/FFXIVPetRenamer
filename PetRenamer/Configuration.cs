using Dalamud.Configuration;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
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

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;
        [NonSerialized]
        private PetRenamerPlugin plugin;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
            this.plugin = PluginLink.PetRenamerPlugin;

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
            if (PluginInterface == null) return;
            this.PluginInterface?.SavePluginConfig(this);
            Globals.RedrawPet = true;
        }
    }
}
