#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using PetRenamer.Core.Serialization;
using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer3 : ILegacyStepperElement
{
    public int OldVersion { get; } = 3;

    public void Upgrade(Configuration configuration)
    {
        List<SerializableUserV2> newUsers = new List<SerializableUserV2>();

        foreach(SerializableUser oldUser in configuration.serializableUsers!)
        {
            List<int> ids = new List<int>();
            List<string> names = new List<string>();
            foreach(SerializableNickname nickname in oldUser.nicknames)
            {
                ids.Add(nickname.ID);
                names.Add(nickname.Name);
            }
            newUsers.Add(new SerializableUserV2(ids.ToArray(), names.ToArray(), oldUser.username, oldUser.homeworld));
        }

        configuration.serializableUsersV2 = newUsers.ToArray();
        configuration.serializableUsers = null;
        configuration.Version = 4;
        configuration.Save();
    }
}
#pragma warning restore CS0618 // Type or member is obsolete