using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Runtime.InteropServices;
using System;
using FFCompanion = FFXIVClientStructs.FFXIV.Client.Game.Character.Companion;
using GameObjectStruct = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace PetRenamer.Core.Updatable.Updatables
{
    [Updatable]
    internal class PlayerDataGatherer : Updatable
    {
        StringUtils stringUtils;
        NicknameUtils nicknameUtils;

        public PlayerDataGatherer()
        {
            stringUtils = PluginLink.Utils.Get<StringUtils>();
            nicknameUtils = PluginLink.Utils.Get<NicknameUtils>();
        }

        public override unsafe void Update(Framework frameWork)
        {
            GameObjectStruct* me = GameObjectManager.GetGameObjectByIndex(0);
            if (me == null) return;
            byte[] nameBytes = new byte[64];
            IntPtr intPtr = (nint)me->GetName();
            Globals.CurrentUserGender = me->Gender;
            Marshal.Copy(intPtr, nameBytes, 0, PluginConstants.ffxivNameSize);
            Globals.CurrentUserName = PluginLink.Utils.Get<StringUtils>().FromBytes(nameBytes);
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
