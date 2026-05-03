using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IStringHelper
{
    SeString WrapInColor(string petName, Vector3? edgeColor = null, Vector3? textColor= null);
    void ReplaceSeString(ref SeString message, string replaceString, IPetSheetData petData, bool checkForEmptySpace = true, Vector3? edgeColor = null, Vector3? textColor = null);
    //void ReplaceATKString(AtkTextNode* atkNode, string baseString, string replaceString, Vector3? edgeColor, Vector3? textColor);
    void ReplaceATKString(AtkTextNode* atkNode, IPetSheetData? petData, NameType nameType, IPettableUser? user = null);
    void ReplaceATKString(AtkTextNode* atkNode, IPettablePet? pettablePet, NameType nameType);
    string CleanupString(string str);
    string CleanupActionName(string str);
    Vector3? ParseVector3(string? line);
    bool TryParseVector3(string? line, [NotNullWhen(true)] out Vector3? vector3);
    string ToVector3String(Vector3 vector);
}
