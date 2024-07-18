#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
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
#pragma warning restore CS0618 // Type or member is obsolete
