using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Chat.Structs;

internal struct SheetGroup(ref string customName, ref PetSheetData data)
{
    public readonly string CustomName = customName;
    public readonly PetSheetData PetSheetData = data;

    public int CompareTo(SheetGroup other) => PetSheetData.BaseSingular.Length.CompareTo(other.PetSheetData.BaseSingular.Length);
}
