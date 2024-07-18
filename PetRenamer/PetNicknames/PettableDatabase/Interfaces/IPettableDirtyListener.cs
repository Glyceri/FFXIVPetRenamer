using System;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDirtyListener
{
    void RegisterOnDirtyName(Action<INamesDatabase> onNamesDatabase);
    void RegisterOnDirtyEntry(Action<IPettableDatabaseEntry> onEntry);
    void RegisterOnClearEntry(Action<IPettableDatabaseEntry> onEntry);
    void RegisterOnDirtyDatabase(Action<IPettableDatabase> onDatabase);

    void UnregisterOnDirtyName(Action<INamesDatabase> onNamesDatabase);
    void UnregisterOnDirtyEntry(Action<IPettableDatabaseEntry> onEntry);
    void UnregisterOnClearEntry(Action<IPettableDatabaseEntry> onEntry);
    void UnregisterOnDirtyDatabase(Action<IPettableDatabase> onDatabase);
}
