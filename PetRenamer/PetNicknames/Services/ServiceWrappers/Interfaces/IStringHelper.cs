using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IStringHelper
{
    public SeString WrapInColor(string petName, Vector3? edgeColor = null, Vector3? textColor= null);
    public SeString? ReplaceStringPart(string baseString, string replaceString, IPetSheetData petData, bool checkForEmptySpaces = true, Vector3? edgeColor = null, Vector3? textColor = null);
    public void ReplaceSeString(ref SeString message, string replaceString, IPetSheetData petData, bool checkForEmptySpace = true, Vector3? edgeColor = null, Vector3? textColor = null);
    public string ReplaceATKString(AtkTextNode* atkNode, string baseString, string replaceString, Vector3? edgeColor, Vector3? textColor, IPetSheetData petData, bool checkForEmptySpace = true);
    public string CleanupString(string str);
    public string CleanupActionName(string str);
    public Vector3? ParseVector3(string? line);
    public bool TryParseVector3(string? line, [NotNullWhen(true)] out Vector3? vector3);
    public string ToVector3String(Vector3 vector);
}
