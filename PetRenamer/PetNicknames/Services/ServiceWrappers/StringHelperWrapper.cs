using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class StringHelperWrapper : IStringHelper
{
    public unsafe string ReplaceATKString(AtkTextNode* atkNode, string baseString, string replaceString, IPetSheetData petData, bool checkForEmptySpace = true)
    {
        if (atkNode == null) return baseString;
        string newString = ReplaceStringPart(baseString, replaceString, petData, checkForEmptySpace);
        return SetATKString(atkNode, newString);
    }

    public unsafe string SetATKString(AtkTextNode* atkNode, string text)
    {
        string newString = text + '\0';

        byte[] data = Encoding.UTF8.GetBytes(newString);

        atkNode->NodeText.SetString(data);
        return text;
    }

    public void ReplaceSeString(ref SeString message, string replaceString, IPetSheetData petData, bool checkForEmptySpace = true)
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

    public string ReplaceStringPart(string baseString, string replaceString, IPetSheetData petData, bool checkForEmptySpaces = true)
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
            baseString = Regex.Replace(baseString, regString, MakeString(PluginConstants.forbiddenCharacter, i + 1), RegexOptions.IgnoreCase);
        }

        for (int i = length - 1; i >= 0; i--)
        {
            string part = parts[i];
            if (part == string.Empty) continue;
            baseString = baseString.Replace(MakeString(PluginConstants.forbiddenCharacter, i + 1), replaceString);
        }

        return baseString;
    }

    List<string> GetString(IPetSheetData petData)
    {
        // Can be simplified it said... this is cursed
        List<string> parts = [.. petData.Plural, .. petData.Singular];

        parts.Sort((s1, s2) => s1.Length.CompareTo(s2.Length));
        parts.Reverse();

        return parts;
    }

    string MakeString(char c, int count) => new string(c, count);

    public string CleanupString(string str)
    {
        return str.Replace("サモン・", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("Summon ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("Invocation ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("-Beschwörung", string.Empty, StringComparison.InvariantCultureIgnoreCase);
    }

    public string CleanupActionName(string str)
    {
        return str.Replace("カーバンクル・", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("・エギ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("-Egi", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace(" Carbuncle", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("Carbuncle ", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                  .Replace("-Karfunkel", string.Empty, StringComparison.InvariantCultureIgnoreCase);        
    }
}
