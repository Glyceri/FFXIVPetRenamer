using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Services;

internal class EnablableRegistration
{
    public readonly IEnablableHandler EnablableHandler;
    public readonly string            TitleTranslatorKey;
    public readonly string            DescriptionTranslatorKey;
    
    public EnablableRegistration(IEnablableHandler handler, string translatorKey)
    {
        EnablableHandler         = handler;
        TitleTranslatorKey       = $"Enablable.{translatorKey}.Title";
        DescriptionTranslatorKey = $"Enablable.{translatorKey}.Description";
    }
}