#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using PetRenamer.Core.Serialization;
using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer4 : ILegacyStepperElement
{
    public int OldVersion { get; } = 4;

    public void Upgrade(Configuration configuration)
    {
        List<SerializableUserV3> newUsers = new List<SerializableUserV3>();

        foreach (SerializableUserV2 oldUser in configuration.serializableUsersV2!)
        {
            newUsers.Add(new SerializableUserV3(oldUser.ids, oldUser.names, oldUser.username, oldUser.homeworld, [-411, -417, -416, -415, -407], [-411, -417, -416, -415, -407]));
        }

        configuration.serializableUsersV3 = newUsers.ToArray();
        configuration.serializableUsersV2 = null;
        configuration.Version = 5;
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
