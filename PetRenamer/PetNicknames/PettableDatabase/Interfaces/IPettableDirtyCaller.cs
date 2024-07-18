
namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDirtyCaller
{
    void DirtyName(in INamesDatabase nameDatabase);
    void DirtyEntry(in IPettableDatabaseEntry entry);
    void ClearEntry(in IPettableDatabaseEntry entry);
    void DirtyDatabase(in IPettableDatabase database);
}
