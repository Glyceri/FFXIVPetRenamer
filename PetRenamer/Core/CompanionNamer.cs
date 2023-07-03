using FFCompanion = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using GameObjectStruct = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;
using System.Runtime.InteropServices;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core
{
    public class CompanionNamer
    {
        StringUtils stringUtils;
        NicknameUtils nicknameUtils;

        public CompanionNamer()
        {
            stringUtils = PluginLink.Utils.Get<StringUtils>();
            nicknameUtils = PluginLink.Utils.Get<NicknameUtils>();
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

            if (!PluginLink.Configuration.displayCustomNames) return;
            if (!nicknameUtils.Contains(Globals.CurrentID)) return;

            Globals.CurrentName = stringUtils.GetName(Globals.CurrentID);

            string usedName = Globals.CurrentName;

            byte* name = playerCompanion->Character.GameObject.GetName();
            Marshal.Copy(stringUtils.GetBytes(usedName), 0, (nint)name, PluginConstants.ffxivNameSize);


            if (Globals.RedrawPet)
            {
                playerCompanion->Character.GameObject.DisableDraw();
                playerCompanion->Character.GameObject.EnableDraw();
            }

            Globals.RedrawPet = false;
        }
    }
}
