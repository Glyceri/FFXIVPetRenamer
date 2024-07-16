using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class HousekeepingHook : HookableElement, IHousekeepingHook
{
    public HousekeepingHook(DalamudServices services, IPettableUserList userList, IPetServices petServices, IPettableDirtyListener dirtyListener) : base(services, userList, petServices, dirtyListener)
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

    protected override void OnDispose()
    {
        DalamudServices.ClientState.EnterPvP -= OnPVPEnter;
    }
}
