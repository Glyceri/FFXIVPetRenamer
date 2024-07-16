#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
#pragma warning disable CS0612 // Type or member is obsolete
using PetRenamer.Core.Serialization;
using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using System.Collections.Generic;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer7 : ILegacyStepperElement
{
    readonly PetServices petServices;

    public int OldVersion { get; } = 7;

    public LegacyNamingVer7(PetServices petServices)
    {
        this.petServices = petServices;
    }

    // For the LONGEST TIME in pet nicknames you could only set 1 nickname per job, this all changed in ... idk, but savefile version  8!
    public void Upgrade(Configuration configuration)
    {

        List<SerializableUserV3> newSerializableUsers = new List<SerializableUserV3>();

        if (configuration.serializableUsersV3 != null)
        {
            foreach (SerializableUserV3 oldUser in configuration.serializableUsersV3)
            {
                List<int> newIDS = new List<int>();
                List<string> newNames = new List<string>();
                for (int i = 0; i < oldUser.ids.Length; i++) 
                {
                    int id = oldUser.ids[i];
                    string name = oldUser.names[i];

                    if (id > -1) 
                    { 
                        newIDS.Add(id); 
                        newNames.Add(name);
                        continue;
                    }


                    int[] remappedIds = petServices.PetSheets.GetObsoleteIDsFromClass(id);
                    foreach(int remappedId in remappedIds)
                    {
                        newIDS.Add(remappedId);
                        newNames.Add(name);
                    }
                }
                newSerializableUsers.Add(new SerializableUserV3(newIDS.ToArray(), newNames.ToArray(), oldUser.username, oldUser.homeworld, oldUser.mainSkeletons, oldUser.softSkeletons));
            }
        }

        configuration.serializableUsersV3 = newSerializableUsers.ToArray();

        configuration.Version = 8;
    }
}
#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
