using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer8 : ILegacyStepperElement
{
    public int OldVersion { get; } = 8;

    public void Upgrade(Configuration configuration)
    {
        // I thought there would be SOMETHING transferrable from old, there isn't
        // It's still good practise for myself to set the save version to 9 because
        // I don't want people to save with the old file format as I dont support that anymore

        configuration.Version = 9;
    }
}
