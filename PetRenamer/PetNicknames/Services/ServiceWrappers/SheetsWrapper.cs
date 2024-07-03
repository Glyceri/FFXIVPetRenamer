using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class SheetsWrapper : IPetSheets
{
    public ExcelSheet<Companion>? petSheet { get; init; }
    public ExcelSheet<Pet>? battlePetSheet { get; init; }
    public ExcelSheet<World>? worlds { get; init; }
    public ExcelSheet<Race>? races { get; init; }
    public ExcelSheet<ClassJob>? classJob { get; init; }
    public ExcelSheet<Action>? actions { get; init; }
    public ExcelSheet<TextCommand>? textCommands { get; init; }

    public SheetsWrapper(DalamudServices dalamudServices)
    {
        petSheet = dalamudServices.DataManager.GetExcelSheet<Companion>();
        worlds = dalamudServices.DataManager.GetExcelSheet<World>();
        races = dalamudServices.DataManager.GetExcelSheet<Race>();
        classJob = dalamudServices.DataManager.GetExcelSheet<ClassJob>();
        battlePetSheet = dalamudServices.DataManager.GetExcelSheet<Pet>();
        actions = dalamudServices.DataManager.GetExcelSheet<Action>();
        textCommands = dalamudServices.DataManager.GetExcelSheet<TextCommand>();
    }

    public TextCommand? GetCommand(uint id) => textCommands?.GetRow(id);
    public Action? GetAction(uint actionID) => actions?.GetRow(actionID);

    public string? GetClassName(int id)
    {
        if (classJob == null) return null;
        foreach (ClassJob cls in classJob)
            if (cls.RowId == id)
                return cls.Name;
        return null;
    }

    public string? GetWorldName(ushort worldID)
    {
        if (worlds == null) return null;
        World? world = worlds.GetRow(worldID);
        if (world == null) return null;
        return world.InternalName;
    }
}
