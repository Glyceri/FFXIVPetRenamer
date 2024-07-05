using PetRenamer.Legacy.LegacyStepper.LegacyElements;
using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using System.Collections.Generic;

namespace PetRenamer.Legacy.LegacyStepper;

internal class LegacyStepper
{
    readonly Configuration Configuration;
    readonly PetServices PetServices;

    readonly List<ILegacyStepperElement> legacyStepperElements = new List<ILegacyStepperElement>();

    public LegacyStepper(Configuration configuration, PetServices petServices)
    {
        PetServices = petServices;
        Configuration = configuration;

        if (Configuration.Version <= 2 || Configuration.Version > Configuration.currentSaveFileVersion)
        {
            // Actually hopeless to try and update...
            // You actually used this plogon on day one (I know you didn't)
            // Or you edited your savefile improperly, to hell with it.
            // Let's set it to the max and hope for the best...
            Configuration.Version = Configuration.currentSaveFileVersion;
            return;
        }

        // These entries need to be in order... this system isn't meant to be pretty!
        // But I do care that old users can keep their nicknames
        legacyStepperElements.Add(new LegacyNamingVer3());
        legacyStepperElements.Add(new LegacyNamingVer4());
        legacyStepperElements.Add(new LegacyNamingVer5());
        legacyStepperElements.Add(new LegacyNamingVer6(PetServices));


        foreach(ILegacyStepperElement legacyStepperElement in legacyStepperElements)
        {
            if (legacyStepperElement.OldVersion != Configuration.Version) continue;
            legacyStepperElement.Upgrade(Configuration);
        }
    }

}
