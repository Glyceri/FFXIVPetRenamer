using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Utilization.UtilsModule
{
    internal class SheetUtils : UtilsRegistryType
    {
        ExcelSheet<Companion> petSheet { get; set; } = null!;

        internal override void OnRegistered()
        {
            petSheet = PluginHandlers.DataManager.GetExcelSheet<Companion>()!;
        }

        public string GetCurrentPetName()
        {
            foreach (Companion pet in petSheet)
            {
                if (pet == null) continue;

                if (pet.Model.Value!.Model == Globals.CurrentID)
                    return pet.Singular.ToString();
            }
            return string.Empty;
        }
    }
}
