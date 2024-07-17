﻿using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IStringHelper
{
    string? ReplaceStringPart(string baseString, string replaceString, IPetSheetData petData, bool checkForEmptySpaces = true);
    void ReplaceSeString(ref SeString message, string replaceString, IPetSheetData petData, bool checkForEmptySpace = true);
    string ReplaceATKString(AtkTextNode* atkNode, string baseString, string replaceString, IPetSheetData petData, bool checkForEmptySpace = true);
    string SetATKString(AtkTextNode* atkNode, string text);
    string CleanupString(string str);
    string CleanupActionName(string str);
}
