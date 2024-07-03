using PetRenamer.PetNicknames.PettableDatabase.Interfaces;

namespace PetRenamer.PetNicknames.PettableDatabase;

internal class PettableNameDatabase : INamesDatabase
{
    public int[] IDs { get; } = new int[0];
    public string[] Names { get; } = new string[0];

    public PettableNameDatabase(int[] ids, string[] names)
    {
        Names = names;
        IDs = ids;
    }
}
