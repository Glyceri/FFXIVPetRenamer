using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer5 : ILegacyStepperElement
{
    public int OldVersion { get; } = 5;

    public void Upgrade(Configuration configuration)
    {
        // This used to convert from old settings to new, but in 2.0 this is irrelevant
        // We still need to step up though

        configuration.Version = 6;
    }
}
