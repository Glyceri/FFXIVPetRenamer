#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using PetRenamer.Core.Serialization;
using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Collections.Generic;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer6 : ILegacyStepperElement
{
    readonly PetServices petServices;

    public int OldVersion { get; } = 6;

    public LegacyNamingVer6(PetServices petServices)
    {
        this.petServices = petServices;
    }

    // This one maps old Model.value.Model to Model.value.RowID as this is a uniqute identifier for ALL minions
    // Also this one was bugged in 1.0 since a later version.....
    // I'm just adding it back because I like to think in the back of my mind that everyone should be able to use the plugin with any save at any point
    // This is because back in the day I had to rewrite how I stored data a LOT
    public void Upgrade(Configuration configuration)
    {
        if (configuration.serializableUsersV3 != null)
        {
            List<SerializableUserV3> newSerializableUsers = new List<SerializableUserV3>();

            foreach (SerializableUserV3 user in configuration.serializableUsersV3)
            {
                List<int> newIDs = new List<int>();
                List<string> newnames = new List<string>();
                for (int i = 0; i < user.ids.Length; i++)
                {
                    if (user.ids[i] < -1)
                    {
                        newIDs.Add(user.ids[i]);
                        newnames.Add(user.names[i]);
                        continue;
                    }

                    List<IPetSheetData> legacyPets = petServices.PetSheets.GetLegacyPets(user.ids[i]);

                    foreach(IPetSheetData legacyPetsData in legacyPets)
                    {
                        newIDs.Add(legacyPetsData.Model);
                        newnames.Add(user.names[i]);
                    }
                }

                newSerializableUsers.Add(new SerializableUserV3(newIDs.ToArray(), newnames.ToArray(), user.username, user.homeworld, user.mainSkeletons, user.softSkeletons));
            }

            configuration.serializableUsersV3 = newSerializableUsers.ToArray();

            configuration.Version = 7;
            configuration.Save();
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
