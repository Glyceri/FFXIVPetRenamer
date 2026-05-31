using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
#pragma warning disable CS0612 // Type or member is obsolete

internal class LegacyNamingVer12 : ILegacyStepperElement
{
    public int OldVersion 
        => 12;
    
    public void Upgrade(Configuration configuration)
    {
        configuration.Version         = 13;

        configuration.currentLanguage = (PetNicknamesLanguage)configuration.languageSettings;
    }
}

#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete