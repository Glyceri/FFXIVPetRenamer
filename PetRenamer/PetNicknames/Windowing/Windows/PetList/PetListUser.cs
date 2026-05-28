using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.PetList.Interfaces;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetList;

internal class PetListUser(IPettableDatabaseEntry entry) : IPetListDrawable
{
    public readonly IPettableDatabaseEntry Entry = entry;
}
