using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Chat.Structs;

internal struct SheetGroup(ref string customName, ref IPetSheetData data)
{
    public readonly string CustomName = customName;
    public readonly IPetSheetData PetSheetData = data;

    public int CompareTo(SheetGroup other) => PetSheetData.BaseSingular.Length.CompareTo(other.PetSheetData.BaseSingular.Length);
}
