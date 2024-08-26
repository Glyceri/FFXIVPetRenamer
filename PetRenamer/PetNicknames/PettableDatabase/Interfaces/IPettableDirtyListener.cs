using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDirtyListener
{
    void RegisterOnDirtyName(Action<INamesDatabase> onNamesDatabase);
    void RegisterOnDirtyEntry(Action<IPettableDatabaseEntry> onEntry);
    void RegisterOnClearEntry(Action<IPettableDatabaseEntry> onEntry);
    void RegisterOnDirtyDatabase(Action<IPettableDatabase> onDatabase);
    void RegisterOnPlayerCharacterDirty(Action<IPettableUser> user);

    void UnregisterOnDirtyName(Action<INamesDatabase> onNamesDatabase);
    void UnregisterOnDirtyEntry(Action<IPettableDatabaseEntry> onEntry);
    void UnregisterOnClearEntry(Action<IPettableDatabaseEntry> onEntry);
    void UnregisterOnDirtyDatabase(Action<IPettableDatabase> onDatabase);
    void UnregisterOnPlayerCharacterDirty(Action<IPettableUser> user);
}
