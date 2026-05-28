using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class NameService : INameService
{
    private readonly IStringHelper StringHelper;
    private          IPronounHook? PronounHook;
    
    public NameService(IStringHelper stringHelper)
        => StringHelper = stringHelper;
        
    private string GetRawName(IPetSheetData petData)
        => petData.BaseSingular;
    
    private string? GetPronoun()
        => PronounHook?.LastGottenPronoun;
    
    private string GetActionName(IPetSheetData petData)
        => StringHelper.CleanupString(petData.ActionName);
    
    public string? GetName(NameType nameType, IPetSheetData petData)
        => nameType switch
        {
            NameType.Raw     => GetRawName(petData),
            NameType.Pronoun => GetPronoun(),
            NameType.Action  => GetActionName(petData),
            _                => null
        };

    public void RegisterPronounHook(IPronounHook pronounHook) 
        => PronounHook = pronounHook;
}