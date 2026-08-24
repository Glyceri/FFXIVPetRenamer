using Dalamud.Game.NativeWrapper;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking;

internal abstract class HookableElement : IHookableElement
{
    protected readonly DalamudServices DalamudServices;
    protected readonly IPetServices    PetServices;

    protected HookableElement(DalamudServices services, IPetServices petServices)
    {
        DalamudServices = services;
        PetServices     = petServices;

        PetServices.DirtyListener.RegisterOnDirtyDatabase(OnPettableDatabaseChange);
        PetServices.DirtyListener.RegisterOnClearEntry(OnPettableEntryClear);
        PetServices.DirtyListener.RegisterOnDirtyEntry(OnPettableEntryChange);
        PetServices.DirtyListener.RegisterOnDirtyName(OnNameDatabaseChange);
        PetServices.DirtyListener.RegisterOnPlayerCharacterDirty(OnPlayerDirty);
        PetServices.DirtyListener.RegisterOnDirtyConfig(OnConfigChanged);

        DalamudServices.Hooking.InitializeFromAttributes(this);
    }

    public    abstract void Init();
    protected abstract void OnDispose();

    protected virtual void OnNameDatabaseChange(INamesDatabase nameDatabase)            
        => InternalRefresh();

    protected virtual void OnPettableDatabaseChange(IPettableDatabase pettableDatabase) 
        => InternalRefresh();

    protected virtual void OnPettableEntryChange(IPettableDatabaseEntry pettableEntry)  
        => InternalRefresh();

    protected virtual void OnPettableEntryClear(IPettableDatabaseEntry pettableEntry)   
        => InternalRefresh();

    protected virtual void OnPlayerDirty(IPettableUser user)                           
        => InternalRefresh();

    protected virtual void OnConfigChanged(Configuration _)
        => InternalRefresh();
    
    private void InternalRefresh()
        => DalamudServices.Framework.Run(Refresh);
    
    protected virtual void Refresh() { }

    protected unsafe void RefreshAddon(string addonName, int index = 1)
    {
        AtkUnitBasePtr unitBasePtr = DalamudServices.GameGui.GetAddonByName(addonName, index);
        
        if (unitBasePtr.IsNull)
        {
            return;
        }
        
        AtkUnitBase* unitBase = (AtkUnitBase*)unitBasePtr.Address;

        if (unitBase == null)
        {
            return;
        }
        
        unitBase->OnRequestedUpdate(AtkStage.Instance()->GetNumberArrayData(), AtkStage.Instance()->GetStringArrayData());
    }
    
    protected unsafe void ForceRefreshAddon(string addonName, int index = 1)
    {
        AtkUnitBasePtr unitBasePtr = DalamudServices.GameGui.GetAddonByName(addonName, index);
        
        if (unitBasePtr.IsNull)
        {
            return;
        }
        
        AtkUnitBase* unitBase = (AtkUnitBase*)unitBasePtr.Address;

        if (unitBase == null)
        {
            return;
        }
        
        unitBase->OnRefresh(unitBase->AtkValuesCount, unitBase->AtkValues);
    }
    
    public void Dispose()
    {
        PetServices.DirtyListener.UnregisterOnDirtyDatabase(OnPettableDatabaseChange);
        PetServices.DirtyListener.UnregisterOnClearEntry(OnPettableEntryClear);
        PetServices.DirtyListener.UnregisterOnDirtyEntry(OnPettableEntryChange);
        PetServices.DirtyListener.UnregisterOnDirtyName(OnNameDatabaseChange);
        PetServices.DirtyListener.UnregisterOnPlayerCharacterDirty(OnPlayerDirty);

        OnDispose();
    }
}
