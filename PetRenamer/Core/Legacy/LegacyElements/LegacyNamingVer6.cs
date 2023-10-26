#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Legacy.Attributes;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using System.Collections.Generic;

namespace PetRenamer.Core.Legacy.LegacyElements;

[Legacy(new int[1] { 6 })]
internal class LegacyNamingVer6 : LegacyElement
{
    internal override void OnPlayerAvailable(int detectedVersion, ref PlayerCharacter player)
    {
        if (detectedVersion != 6) return;

        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
        {
            List<SerializableNickname> nicknames = new List<SerializableNickname>();
            for (int i = 0; i < user.SerializableUser.length; i++)
            {
                foreach (Companion pet in SheetUtils.instance.petSheet)
                    if (pet.Model!.Value!.Model == user.SerializableUser[i].ID)
                        nicknames.Add(new SerializableNickname((int)pet.Model!.Value!.RowId, user.SerializableUser[i].Name));
                if (user.SerializableUser[i].ID < -1) 
                    nicknames.Add(new SerializableNickname(user.SerializableUser[i].ID, user.SerializableUser[i].Name));
            }
            user.SerializableUser.Reset();
            foreach (SerializableNickname nickname in nicknames)
                user.SerializableUser.SaveNickname(nickname.ID, nickname.Name, true);
        }

        PluginLink.Configuration.Version = 7;
        PluginLink.Configuration.Save();
    }
}
#pragma warning restore CS0618 // Type or member is obsolete