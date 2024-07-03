namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface INamesDatabase
{
    public int[] IDs { get; }
    public string[] Names { get; }
}
