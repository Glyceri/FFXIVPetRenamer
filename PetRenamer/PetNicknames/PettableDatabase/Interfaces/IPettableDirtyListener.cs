using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Hooking.Enum;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Enums;
using System;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDirtyListener
{
    public void RegisterOnDirtyName(Action<INamesDatabase> onNamesDatabase);
    public void RegisterOnDirtyEntry(Action<IPettableDatabaseEntry> onEntry);
    public void RegisterOnClearEntry(Action<IPettableDatabaseEntry> onEntry);
    public void RegisterOnDirtyDatabase(Action<IPettableDatabase> onDatabase);
    public void RegisterOnPlayerCharacterDirty(Action<IPettableUser> user);
    public void RegisterOnConfigurationDirty(Action<Configuration> configuration);
    public void RegisterOnPetModeDirty(Action<PetWindowMode> petWindowMode);
    public void RegisterOnWindowDirty(Action windowDirty);
    public void RegisterOnDirtyNavigation(NavigationDirty dirtyNavigation);

    public void UnregisterOnDirtyName(Action<INamesDatabase> onNamesDatabase);
    public void UnregisterOnDirtyEntry(Action<IPettableDatabaseEntry> onEntry);
    public void UnregisterOnClearEntry(Action<IPettableDatabaseEntry> onEntry);
    public void UnregisterOnDirtyDatabase(Action<IPettableDatabase> onDatabase);
    public void UnregisterOnPlayerCharacterDirty(Action<IPettableUser> user);
    public void UnregisterOnConfigurationDirty(Action<Configuration> configuration);
    public void UnregisterOnPetModeDirty(Action<PetWindowMode> petWindowMode);
    public void UnregisterOnWindowDirty(Action windowDirty);
    public void UnregisterOnDirtyNavigation(NavigationDirty dirtyNavigation);

    public delegate bool NavigationDirty(nint atkUnitBase, NavigationInputId inputId, AtkEventData.AtkInputData.InputState inputState);

}
