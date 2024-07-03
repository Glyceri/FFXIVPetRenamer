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

    public string? GetName(int ID)
    {
        for (int i = 0; i < IDs.Length; i++)
        {
            if (IDs[i] != ID) continue;
            return Names[i];
        }
        return null;
    }

    public void SetName(int ID, string? name)
    {
        throw new System.NotImplementedException();
    }
}
