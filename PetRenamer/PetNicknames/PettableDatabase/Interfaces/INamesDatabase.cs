using PetRenamer.PetNicknames.Serialization;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface INamesDatabase
{
    public int[] IDs { get; }
    public string[] Names { get; }
    string? GetName(int ID);
    void SetName(int ID, string? name);
    bool IsDirty { get; }

    void MarkDirtyAsNoticed();

    SerializableNameData SerializeData();
}
