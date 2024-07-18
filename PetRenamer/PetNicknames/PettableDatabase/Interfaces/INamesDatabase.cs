using PN.S;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface INamesDatabase
{
    public int[] IDs { get; }
    public string[] Names { get; }
    public int Length { get; }
    string? GetName(int ID);
    void SetName(int ID, string? name);

    void Update(int[] IDs, string[] names, IPettableDirtyCaller dirtyCaller);

    SerializableNameData SerializeData();
}
