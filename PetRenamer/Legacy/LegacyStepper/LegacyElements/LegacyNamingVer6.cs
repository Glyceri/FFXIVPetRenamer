#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using PetRenamer.Core.Serialization;
using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
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

    public void Upgrade(Configuration configuration)
    {
        if (configuration.serializableUsersV3 != null)
        {
            foreach (SerializableUserV3 user in configuration.serializableUsersV3)
            {
                List<SerializableNickname> nicknames = new List<SerializableNickname>();
                for (int i = 0; i < user.ids.Length; i++)
                {
                    if (user.ids[i] < -1)
                    {
                        nicknames.Add(new SerializableNickname(user.ids[i], user.names[i]));
                        continue;
                    }

                    IPetSheetData? data = petServices.PetSheets.GetPet(user.ids[i]);
                    if (data == null) continue;

                    nicknames.Add(new SerializableNickname(data.Model, user.names[i]));
                }
            }

            configuration.Version = 7;
            configuration.Save();
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
