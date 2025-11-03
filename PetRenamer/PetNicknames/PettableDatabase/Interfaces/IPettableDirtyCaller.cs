using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Enums;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDirtyCaller
{
    public void DirtyName(in INamesDatabase nameDatabase);
    public void DirtyEntry(in IPettableDatabaseEntry entry);
    public void ClearEntry(in IPettableDatabaseEntry entry);
    public void DirtyDatabase(in IPettableDatabase database);
    public void DirtyPlayer(IPettableUser user);
    public void DirtyConfiguration(Configuration configuration);
    public void DirtyPetMode(PetWindowMode petMode);
    public void DirtyWindow();
}
