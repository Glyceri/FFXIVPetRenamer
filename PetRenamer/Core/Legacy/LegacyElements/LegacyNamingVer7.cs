#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using Dalamud.Game.ClientState.Objects.SubKinds;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Legacy.Attributes;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using System.Collections.Generic;

namespace PetRenamer.Core.Legacy.LegacyElements;

[Legacy(new int[1] { 7 })]
internal class LegacyNamingVer7 : LegacyElement
{
    internal override void OnPlayerAvailable(int detectedVersion, ref PlayerCharacter player)
    {
        if (detectedVersion != 7) return;

        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
        {
            List<SerializableNickname> nicknames = new List<SerializableNickname>();
            for (int i = 0; i < user.SerializableUser.length; i++)
            {
                int id = user.SerializableUser.ids[i];
                string name = user.SerializableUser.names[i];
                if (id > -1) nicknames.Add(new SerializableNickname(id, name));
                else if (id == -1) continue;

                foreach (KeyValuePair<int, int> kvp in RemapUtils.instance.battlePetToClass)
                {
                    if (kvp.Value != id) continue;
                    nicknames.Add(new SerializableNickname(kvp.Key, name));
                }
            }

            user.SerializableUser.Reset();
            foreach (SerializableNickname nickname in nicknames)
                user.SerializableUser.SaveNickname(nickname.ID, nickname.Name, true);
        }

        PluginLink.Configuration.Version = 8;
        PluginLink.Configuration.Save();
    }
}
#pragma warning restore CS0618 // Type or member is obsolete