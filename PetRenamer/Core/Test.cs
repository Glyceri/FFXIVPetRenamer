using System;
using Dalamud.Game;
using Dalamud.IoC;
using FFCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using FFCompanion = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using GameObjectStruct = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;
using System.Runtime.InteropServices;
using Dalamud.Game.Text.SeStringHandling;

using Lumina.Excel.GeneratedSheets;
using Dalamud.Game.ClientState.Objects.SubKinds;
using PetRenamer.Windows;

namespace PetRenamer.Core
{
    public class Test
    {
        PetRenamerPlugin plugin;

        [PluginService] GameObjectManager gameObjectManager { get; set; }   

        public Test(PetRenamerPlugin basePlugin, SigScanner sigScanner)
        {
            this.plugin = basePlugin;
        }


        public unsafe void Update(Dalamud.Game.Framework frameWork)
        {

            GameObjectStruct* me = GameObjectManager.GetGameObjectByIndex(0);
            FFCompanion* meCompanion = (FFCompanion*)me;

            FFCompanion* playerCompanion = meCompanion->Character.Companion.CompanionObject;
            if (playerCompanion == null) return;

            if (plugin.petName[0] == 0) return;

            byte* name = playerCompanion->Character.GameObject.Name;
            Marshal.Copy(plugin.petName, 0, (nint)name, 64);

            MainWindow.testText = playerCompanion->Character.ModelCharaId.ToString();
        }
    }
}
