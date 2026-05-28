#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using PetRenamer.Core.Serialization;
using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using System.Collections.Generic;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer6 : ILegacyStepperElement
{
    private readonly PetServices PetServices;

    public LegacyNamingVer6(PetServices petServices) 
        => PetServices = petServices;
    
    public int OldVersion 
        => 6;
    
    // This one maps old Model.value.Model to Model.value.RowID as this is a uniqute identifier for ALL minions
    // Also this one was bugged in 1.0 since a later version.....
    // I'm just adding it back because I like to think in the back of my mind that everyone should be able to use the plugin with any save at any point
    // This is because back in the day I had to rewrite how I stored data a LOT
    public void Upgrade(Configuration configuration)
    {
        configuration.Version = 7;
        
        if (configuration.serializableUsersV3 == null)
        {
            return;
        }
        
        List<SerializableUserV3> newSerializableUsers = [];

        foreach (SerializableUserV3 user in configuration.serializableUsersV3)
        {
            List<int>    newIDs   = [];
            List<string> newnames = [];
            
            for (int i = 0; i < user.ids.Length; i++)
            {
                if (user.ids[i] < -1)
                {
                    newIDs.Add(user.ids[i]);
                    newnames.Add(user.names[i]);
                    
                    continue;
                }

                IPetSheetData[] legacyPets = PetServices.PetSheets.GetLegacyPets(user.ids[i]);

                foreach(IPetSheetData legacyPetsData in legacyPets)
                {
                    legacyPetsData.Model.AsLegacy(out int id);

                    newIDs.Add(id);
                    newnames.Add(user.names[i]);
                }
            }

            newSerializableUsers.Add(new SerializableUserV3(newIDs.ToArray(), newnames.ToArray(), user.username, user.homeworld, user.mainSkeletons, user.softSkeletons));
        }

        configuration.serializableUsersV3 = newSerializableUsers.ToArray();
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
