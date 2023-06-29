using Dalamud.Game;
using Dalamud.IoC;
using FFCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;
using FFCompanion = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using GameObjectStruct = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;
using System.Runtime.InteropServices;
using System.Numerics;
using Dalamud.Game.Gui.Toast;

namespace PetRenamer.Core
{
    public class CompanionNamer
    {
        PetRenamerPlugin plugin;

        [PluginService] GameObjectManager gameObjectManager { get; set; }

        Utils utils { get; set; }

        public CompanionNamer(PetRenamerPlugin basePlugin, Utils utils, SigScanner sigScanner)
        {
            this.plugin = basePlugin;
            this.utils = utils;
        }


        public unsafe void Update(Dalamud.Game.Framework frameWork)
        {

            GameObjectStruct* me = GameObjectManager.GetGameObjectByIndex(0);
            if (me == null) return;
            FFCompanion* meCompanion = (FFCompanion*)me;
            if (meCompanion == null) return;

            FFCompanion* playerCompanion = meCompanion->Character.Companion.CompanionObject;
            int lastID = Globals.CurrentID;
            Globals.CurrentID = -1;
            Globals.CurrentName = string.Empty;
            if (playerCompanion == null) return;



            int id = playerCompanion->Character.CharacterData.ModelSkeletonId;
            if (lastID != id) Globals.CurrentIDChanged = true;
            
            Globals.CurrentID = id;

            if (!plugin.Configuration.displayCustomNames) return;
            if (!utils.Contains(Globals.CurrentID)) return;

            Globals.CurrentName = utils.GetName(Globals.CurrentID);

            string usedName = Globals.CurrentName;

            byte* name = playerCompanion->Character.GameObject.GetName();

            //if (!plugin.Configuration.displayCustomNames)
                //usedName = utils.MakeTitleCase(utils.GetCurrentPetName());
            Marshal.Copy(utils.GetBytes(usedName), 0, (nint)name, 64);


            if (Globals.RedrawPet)
            {
                playerCompanion->Character.GameObject.DisableDraw();
                playerCompanion->Character.GameObject.EnableDraw();
            }

            Globals.RedrawPet = false;
        }
    }
}
