using PetRenamer.PetNicknames.PettableUsers.Interfaces;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IDirtyCaller
{
    void DirtyName(in INamesDatabase nameDatabase);
    void DirtyEntry(in IPettableDatabaseEntry entry);
    void ClearEntry(in IPettableDatabaseEntry entry);
    void DirtyDatabase(in IPettableDatabase database);
    void DirtyPlayer(IPettableUser user);
}
