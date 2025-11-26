#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
#pragma warning disable CS0612 // Type or member is obsolete

using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PN.S;
using System.Collections.Generic;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer10 : ILegacyStepperElement
{
    private readonly IPetServices PetServices;

    public int OldVersion { get; } = 10;

    public LegacyNamingVer10(IPetServices petServices)
    {
        PetServices = petServices;
    }

    public void Upgrade(Configuration configuration)
    {
        if (configuration.SerializableUsersV5 != null)
        {
            List<SerializableUserV6> newSerializableUsers = [];

            foreach (SerializableUserV5 user in configuration.SerializableUsersV5)
            {
                PetSkeleton[] petSkeletons = PetSkeletonHelper.AsPetSkeletons(user.SoftSkeletonData);

                PetSkeletonHelper.AsMappedArray(petSkeletons, out int[] ids, out int[] skeletonTypes);

                newSerializableUsers.Add(new SerializableUserV6(user.ContentID, user.Name, user.Homeworld, ids, skeletonTypes, Create(user.SerializableNameDatas)));
            }

            configuration.SerializableUsersV6 = newSerializableUsers.ToArray();
            configuration.SerializableUsersV5 = [];
        }

        configuration.Version = 11;
    }

    private SerializableNameDataV3[] Create(SerializableNameDataV2[] oldData)
    {
        int arrayLength = oldData.Length;

        SerializableNameDataV3[] newData = new SerializableNameDataV3[arrayLength];

        for (int i = 0; i < arrayLength; i++)
        {
            newData[i] = new SerializableNameDataV3(oldData[i]);
        }

        return newData;
    }
}

#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
