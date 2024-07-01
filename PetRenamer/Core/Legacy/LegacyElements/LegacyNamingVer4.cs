#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using Dalamud.Game.ClientState.Objects.SubKinds;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Legacy.Attributes;
using PetRenamer.Core.Serialization;
using System.Collections.Generic;

namespace PetRenamer.Core.Legacy.LegacyElements;

[Legacy(new int[1] { 4 })]
internal class LegacyNamingVer4 : LegacyElement
{
    internal override void OnPlayerAvailable(int detectedVersion, ref IPlayerCharacter player)
    {
        if (detectedVersion != 4) return;

        List<SerializableUserV3> newSerializableUsers = new List<SerializableUserV3>();

        foreach (SerializableUserV2 userOld in PluginLink.Configuration.serializableUsersV2!)
            PluginLink.PettableUserHandler.DeclareUser(new SerializableUserV3(userOld.ids, userOld.names, userOld.username, userOld.homeworld), PettableUserSystem.Enums.UserDeclareType.Add, true);

        PluginLink.Configuration.serializableUsersV3 = newSerializableUsers.ToArray();
        PluginLink.Configuration.serializableUsersV2 = null;
        PluginLink.Configuration.Version = 5;
        PluginLink.Configuration.Save();
    }
}
#pragma warning restore CS0618 // Type or member is obsolete