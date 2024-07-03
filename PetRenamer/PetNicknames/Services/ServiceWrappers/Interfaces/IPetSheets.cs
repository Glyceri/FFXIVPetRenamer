﻿using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetSheets
{
    ExcelSheet<Companion>?               petSheet               { get; init; }
    ExcelSheet<Pet>?                     battlePetSheet         { get; init; }
    ExcelSheet<World>?                   worlds                 { get; init; }
    ExcelSheet<Race>?                    races                  { get; init; }
    ExcelSheet<ClassJob>?                classJob               { get; init; }
    ExcelSheet<Action>?                  actions                { get; init; }
    ExcelSheet<TextCommand>?             textCommands           { get; init; }


    TextCommand? GetCommand(uint id);
    Action? GetAction(uint actionID);
    string? GetClassName(int id);
    string? GetWorldName(ushort worldID);
}