using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class StringHelperWrapper : IStringHelper
{
    public unsafe string ReplaceATKString(AtkTextNode* atkNode, string baseString, string replaceString, PetSheetData petData, bool checkForEmptySpace = true)
    {
        if (atkNode == null) return baseString;
        string newString = ReplaceStringPart(baseString, replaceString, petData, checkForEmptySpace);
        atkNode->NodeText.SetString(newString);
        return newString;
    }

    public void ReplaceSeString(ref SeString message, string replaceString, PetSheetData petData, bool checkForEmptySpace = true)
    {
        if (message == null) return;
        for (int i = 0; i < message.Payloads.Count; i++)
        {
            if (message.Payloads[i] is not TextPayload tPayload) continue;

            string curString = tPayload.Text!.ToString();
            tPayload.Text = ReplaceStringPart(curString, replaceString, petData, checkForEmptySpace);

            message.Payloads[i] = tPayload;
        }
    }

    public string ReplaceStringPart(string baseString, string replaceString, PetSheetData petData, bool checkForEmptySpaces = true)
    {
        List<string> parts = GetString(petData);

        int length = parts.Count;

        for (int i = 0; i < length; i++)
        {
            string part = parts[i];
            if (part == string.Empty) continue;
            part = part.Replace("[", @"^\[").Replace("]", @"^\]\");
            string regString = part;
            if (checkForEmptySpaces) regString = $"\\b" + regString + "\\b";
            baseString = Regex.Replace(baseString, regString, MakeString(PluginConstants.forbiddenCharacter, (i + 1)), RegexOptions.IgnoreCase);
        }

        for (int i = length - 1; i >= 0; i--)
        {
            string part = parts[i];
            if (part == string.Empty) continue;
            baseString = baseString.Replace(MakeString(PluginConstants.forbiddenCharacter, (i + 1)), replaceString);
        }

        return baseString;
    }

    List<string> GetString(PetSheetData petData)
    {
        List<string> parts = new List<string>();
        parts.AddRange(petData.Plural);
        parts.AddRange(petData.Singular);

        parts.Sort((s1, s2) => s1.Length.CompareTo(s2.Length));
        parts.Reverse();

        return parts;
    }

    string MakeString(char c, int count) => new string(c, count);
}
