using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.PetList.Interfaces;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetList;

internal class PetListPet : IPetListDrawable
{
    public readonly IPetSheetData PetSheetData;
    public string CustomName;
    public string TempName;

    public PetListPet(in DalamudServices dalamudServices, in IPetSheetData sheetData, string? customName)
    {
        PetSheetData = sheetData;
        CustomName = customName ?? string.Empty;
        TempName = CustomName;
    }

    public void Dispose() { }
}
