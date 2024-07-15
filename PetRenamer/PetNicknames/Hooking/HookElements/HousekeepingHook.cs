using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class HousekeepingHook : HookableElement, IHousekeepingHook
{
    public HousekeepingHook(DalamudServices services, IPettableUserList userList, IPetServices petServices) : base(services, userList, petServices)
    {
    }

    public override void Init()
    {
        DalamudServices.ClientState.EnterPvP += OnPVPEnter;
    }

    void OnPVPEnter()
    {
        DalamudServices.ChatGui.PrintError(Translator.GetLine("PVPWarning"));
    }

    public override void Dispose()
    {
        DalamudServices.ClientState.EnterPvP -= OnPVPEnter;
    }
}
