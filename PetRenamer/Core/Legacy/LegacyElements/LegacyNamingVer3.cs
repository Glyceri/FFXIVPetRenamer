#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
#pragma warning disable CS0612 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Legacy.Attributes;
using PetRenamer.Core.Serialization;
using System.Collections.Generic;

namespace PetRenamer.Core.Legacy.LegacyElements;

[Legacy(new int[1] { 3 })]
internal class LegacyNamingVer3 : LegacyElement
{
    internal override void OnStartup(int detectedVersion)
    {
        if (detectedVersion != 3) return;

        List<SerializableUserV2> newSerializableUsers = new List<SerializableUserV2>();

        foreach (SerializableUser userOld in PluginLink.Configuration.serializableUsers!)
            newSerializableUsers.Add(new SerializableUserV2((SerializableNickname[])userOld.nicknames.Clone(), userOld.username, userOld.homeworld));

        PluginLink.Configuration.serializableUsersV2 = newSerializableUsers.ToArray();
        PluginLink.Configuration.serializableUsers = new SerializableUser[0];
        PluginLink.Configuration.Version = 4;
        PluginLink.Configuration.Save();
    }
}
#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete