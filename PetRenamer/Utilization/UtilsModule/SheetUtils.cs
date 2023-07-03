using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.Attributes;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class SheetUtils : UtilsRegistryType
{
    ExcelSheet<Companion> petSheet { get; set; } = null!;

    internal override void OnRegistered()
    {
        petSheet = PluginHandlers.DataManager.GetExcelSheet<Companion>()!;
    }

    public string GetCurrentPetName()
    {
        PlayerData? playerData = PluginLink.Utils.Get<PlayerUtils>().GetPlayerData();
        if (playerData == null) return string.Empty;
        if (playerData!.Value.companionData == null) return string.Empty;

        foreach (Companion pet in petSheet)
        {
            if (pet == null) continue;

            if (pet.Model.Value!.Model == playerData!.Value.companionData!.Value.currentModelID)
                return pet.Singular.ToString();
        }
        return string.Empty;
    }
}
