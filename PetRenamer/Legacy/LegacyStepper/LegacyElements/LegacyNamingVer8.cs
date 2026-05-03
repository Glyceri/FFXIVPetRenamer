using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer8 : ILegacyStepperElement
{
    public int OldVersion 
        => 8;

    public void Upgrade(Configuration configuration) 
        => configuration.Version = 9;
}
