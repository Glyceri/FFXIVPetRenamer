using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer5 : ILegacyStepperElement
{
    public int OldVersion 
        => 5;
    
    public void Upgrade(Configuration configuration) 
        => configuration.Version = 6;
}
