using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Windows.PetList.Interfaces;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetList;

internal class PetListUser : IPetListDrawable
{
    public readonly IPettableDatabaseEntry Entry;

    public PetListUser(in DalamudServices dalamudServices, in IPettableDatabaseEntry entry)
    {
        Entry = entry;
    }

    public void Dispose()
    {
        
    }
}
