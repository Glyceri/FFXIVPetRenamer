using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IStringHelper
{
    string? ReplaceStringPart(string baseString, string replaceString, PetSheetData petData, bool checkForEmptySpaces = true);
    void ReplaceSeString(ref SeString message, string replaceString, PetSheetData petData, bool checkForEmptySpace = true);
    string ReplaceATKString(AtkTextNode* atkNode, string baseString, string replaceString, PetSheetData petData, bool checkForEmptySpace = true);
    string CleanupString(string str);
    string CleanupActionName(string str);
}
