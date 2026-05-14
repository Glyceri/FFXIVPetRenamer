using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface INameService
{
    string? GetName(NameType nameType, IPetSheetData petData);
    
    void RegisterPronounHook(IPronounHook pronounHook);
}