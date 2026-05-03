#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
#pragma warning disable CS0612 // Type or member is obsolete

using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PN.S;
using System.Collections.Generic;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer9 : ILegacyStepperElement
{
    private readonly IPetServices PetServices;

    public LegacyNamingVer9(IPetServices petServices) 
        => PetServices = petServices;
    
    public int OldVersion
        => 9;

    public void Upgrade(Configuration configuration)
    {
        configuration.Version = 10;
        
        if (configuration.SerializableUsersV4 == null)
        {
            return;
        }
        
        List<SerializableUserV5> newSerializableUsers = [];

        foreach (SerializableUserV4 user in configuration.SerializableUsersV4)
        {
            newSerializableUsers.Add(new SerializableUserV5(user.ContentID, user.Name, user.Homeworld, user.SoftSkeletonData, Create(user.SerializableNameDatas)));
        }

        configuration.SerializableUsersV5 = newSerializableUsers.ToArray();
        configuration.SerializableUsersV4 = [];
    }

    private SerializableNameDataV2[] Create(SerializableNameData[] oldData)
    {
        List<SerializableNameDataV2> newData = [];

        foreach (SerializableNameData old in oldData)
        {
            newData.Add(new SerializableNameDataV2(old));
        }
        
        return newData.ToArray();
    }
}

#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
