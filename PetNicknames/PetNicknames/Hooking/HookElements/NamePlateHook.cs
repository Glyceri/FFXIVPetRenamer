using Dalamud.Game.Gui.NamePlate;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal class NamePlateHook : HookableElement
{
    public NamePlateHook(DalamudServices services, IPetServices petServices, IPettableUserList pettableUserList, IPettableDirtyListener dirtyListener) : base(services, pettableUserList, petServices, dirtyListener) { }

    public override void Init()
    {
        DalamudServices.NameplateGUI.OnNamePlateUpdate += OnPlateUpdate;
        Refresh();
    }

    protected override void OnDispose()
    {
        Refresh();
        DalamudServices.NameplateGUI.OnNamePlateUpdate -= OnPlateUpdate;
    }

    protected override void OnNameDatabaseChange(INamesDatabase nameDatabase) => Refresh();
    protected override void OnPettableDatabaseChange(IPettableDatabase pettableDatabase) => Refresh();
    protected override void OnPettableEntryChange(IPettableDatabaseEntry pettableEntry) => Refresh();
    protected override void OnPettableEntryClear(IPettableDatabaseEntry pettableEntry) => Refresh();

    void Refresh()
    {
        DalamudServices.NameplateGUI.RequestRedraw();
    }

    void OnPlateUpdate(INamePlateUpdateContext context, IReadOnlyList<INamePlateUpdateHandler> handlers)
    {
        if (!PetServices.Configuration.showOnNameplates) return;

        int size = handlers.Count;
        
        for (int i = 0; i < size; i++)
        {
            INamePlateUpdateHandler handler = handlers[i];
            OnSpecificPlateUpdate(handler);
        }
    }

    void OnSpecificPlateUpdate(INamePlateUpdateHandler handler)
    {
        if (handler.NamePlateKind != NamePlateKind.BattleNpcFriendly && 
            handler.NamePlateKind != NamePlateKind.EventNpcCompanion) return;

        if (handler.GameObject == null) return;
        nint address = handler.GameObject.Address;

        IPettablePet? pPet = UserList.GetPet(address);
        if (pPet == null) return;

        string? customPetName = pPet.CustomName;
        if (customPetName == null) return;

        handler.NameParts.Text = customPetName;
    }
}
