namespace PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;

internal interface ILegacyStepperElement
{
    int OldVersion { get; }
    void Upgrade(Configuration configuration);
}
