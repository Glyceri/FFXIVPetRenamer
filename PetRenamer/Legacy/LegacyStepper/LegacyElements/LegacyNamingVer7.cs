#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
#pragma warning disable CS0612 // Type or member is obsolete
using PetRenamer.Core.Serialization;
using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer7 : ILegacyStepperElement
{
    private readonly PetServices PetServices;

    public LegacyNamingVer7(PetServices petServices) 
        => PetServices = petServices;
    

    public int OldVersion 
        => 7;
    
    // For the LONGEST TIME in pet nicknames you could only set 1 nickname per job, this all changed in ... idk, but savefile version  8!
    public void Upgrade(Configuration configuration)
    {
        List<SerializableUserV3> newSerializableUsers = [];

        configuration.Version = 8;
        
        if (configuration.serializableUsersV3 == null)
        {
            return;
        }
        
        foreach (SerializableUserV3 oldUser in configuration.serializableUsersV3)
        {
            List<int>    newIDS   = [];
            List<string> newNames = [];
            
            for (int i = 0; i < oldUser.ids.Length; i++) 
            {
                int    id   = oldUser.ids[i];
                string name = oldUser.names[i];

                if (id > -1) 
                { 
                    newIDS.Add(id); 
                    newNames.Add(name);
                    
                    continue;
                }


                PetSkeleton[] remappedIds = PetServices.PetSheets.GetObsoleteIDsFromClass(id);

                foreach(PetSkeleton remappedId in remappedIds)
                {
                    remappedId.AsLegacy(out int legacyId);

                    newIDS.Add(legacyId);
                    newNames.Add(name);
                }
            }
            
            newSerializableUsers.Add(new SerializableUserV3(newIDS.ToArray(), newNames.ToArray(), oldUser.username, oldUser.homeworld, oldUser.mainSkeletons, oldUser.softSkeletons));
        }

        configuration.serializableUsersV3 = newSerializableUsers.ToArray();
    }
}
#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
