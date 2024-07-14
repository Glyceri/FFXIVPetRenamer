using PN.S;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface INamesDatabase
{
    public int[] IDs { get; }
    public string[] Names { get; }
    public int Length { get; }
    string? GetName(int ID);
    void SetName(int ID, string? name);
    bool IsDirty { get; }
    bool IsDirtyForUI { get; }

    void MarkDirtyAsNoticed();
    void MarkDirtyUIAsNotified();

    void Update(int[] IDs, string[] names);

    SerializableNameData SerializeData();
}
