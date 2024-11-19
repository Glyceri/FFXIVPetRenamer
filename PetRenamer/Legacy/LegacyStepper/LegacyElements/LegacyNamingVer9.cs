#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
#pragma warning disable CS0612 // Type or member is obsolete

using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PN.S;
using System.Collections.Generic;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer9 : ILegacyStepperElement
{
    readonly IPetServices PetServices;

    public int OldVersion { get; } = 9;

    public LegacyNamingVer9(IPetServices petServices)
    {
        PetServices = petServices;
    }

    public void Upgrade(Configuration configuration)
    {

        if (configuration.SerializableUsersV4 != null)
        {
            List<SerializableUserV5> newSerializableUsers = new List<SerializableUserV5>();

            foreach (SerializableUserV4 user in configuration.SerializableUsersV4)
            {
                newSerializableUsers.Add(new SerializableUserV5(user.ContentID, user.Name, user.Homeworld, user.SoftSkeletonData, Create(user.SerializableNameDatas)));
            }

            configuration.SerializableUsersV5 = newSerializableUsers.ToArray();
            configuration.SerializableUsersV4 = [];
        }

        configuration.Version = 10;
    }

    SerializableNameDataV2[] Create(SerializableNameData[] oldData)
    {
        List<SerializableNameDataV2> newData = new List<SerializableNameDataV2>();

        foreach (SerializableNameData old in oldData)
        {
            newData.Add(new SerializableNameDataV2(old));
        }
        return newData.ToArray();
    }
}

#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
