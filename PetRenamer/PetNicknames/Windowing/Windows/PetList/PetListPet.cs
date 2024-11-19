using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows.PetList.Interfaces;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetList;

internal class PetListPet : IPetListDrawable
{
    public readonly IPetSheetData PetSheetData;
    public string CustomName;
    public string TempName;
    public Vector3? EdgeColour;
    public Vector3? TextColour;

    public PetListPet(in DalamudServices dalamudServices, in IPetSheetData sheetData, string? customName, Vector3? edgeColour, Vector3? textColour)
    {
        PetSheetData = sheetData;
        CustomName = customName ?? string.Empty;
        TempName = CustomName;
        EdgeColour = edgeColour;
        TextColour = textColour;
    }

    public void Dispose() { }
}
