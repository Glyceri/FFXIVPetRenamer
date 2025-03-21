using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Numerics;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IStringHelper
{
    SeString WrapInColor(string petName, Vector3? edgeColor = null, Vector3? textColor= null);
    string ToTitleCase(string str);
    SeString? ReplaceStringPart(string baseString, string replaceString, IPetSheetData petData, bool checkForEmptySpaces = true, Vector3? edgeColor = null, Vector3? textColor = null);
    void ReplaceSeString(ref SeString message, string replaceString, IPetSheetData petData, bool checkForEmptySpace = true, Vector3? edgeColor = null, Vector3? textColor = null);
    string ReplaceATKString(AtkTextNode* atkNode, string baseString, string replaceString, Vector3? edgeColor, Vector3? textColor, IPetSheetData petData, bool checkForEmptySpace = true);
    string CleanupString(string str);
    string CleanupActionName(string str);
}
