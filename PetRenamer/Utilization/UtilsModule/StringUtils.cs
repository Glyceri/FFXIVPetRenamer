using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class StringUtils : UtilsRegistryType, ISingletonBase<StringUtils>
{
    public static StringUtils instance { get; set; } = null!;
    public string MakeTitleCase(string str) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());

    public void ReplaceSeString(ref SeString message, ref (string, string)[] validNames, bool checkForEmptySpace = true)
    {
        if (validNames.Length == 0) return;
        if (message == null) return;
        for (int i = 0; i < message.Payloads.Count; i++)
        {
            if (message.Payloads[i] is not TextPayload tPayload) continue;

            string curString = tPayload.Text!.ToString();
            ReplaceString(ref curString, ref validNames, checkForEmptySpace);
            tPayload.Text = curString;

            message.Payloads[i] = tPayload;
        }
    }

    public void ReplaceSeString(ref SeString message, string baseString, string replaceString)
    {
        (string, string)[] strs = new (string, string)[] { (baseString, replaceString) };
        ReplaceSeString(ref message, ref strs);
    }

    public void ReplaceString(ref string baseString, string standard, string replacer, bool checkForEmptySpace = true)
    {
        (string, string)[] strs = new (string, string)[] { (standard, replacer) };
        ReplaceString(ref baseString, ref strs, checkForEmptySpace);
    }

    public void ReplaceString(ref string baseString, ref (string, string)[] validNames, bool checkForEmptySpace = true)
    {
        int counter = 1;

        int length = validNames.Length;
        for(int i = 0; i < length; i++)
        {
            (string, string) str = validNames[i];
            if (str.Item1 == string.Empty || str.Item2 == string.Empty) continue;
            SanitizeString(ref baseString, str.Item2, ++counter, checkForEmptySpace);
            SanitizeString(ref baseString, str.Item1, ++counter, checkForEmptySpace);
        }
        for(int i = length - 1; i >= 0; i--)
        {
            (string, string) str = validNames[i];
            if (str.Item1 == string.Empty || str.Item2 == string.Empty) continue;
            baseString = baseString.Replace(MakeString(PluginConstants.forbiddenCharacter, counter--), str.Item2);
            baseString = baseString.Replace(MakeString(PluginConstants.forbiddenCharacter, counter--), str.Item2);
        }
    }

    public void SanitizeString(ref string baseString, string finder, int count, bool checkForEmptySpace = true)
    {
        int length = PluginConstants.removeables.Length;
        for(int i = 0; i < length; i++)
        {
            string filler = PluginConstants.removeables[i];
            string newFinder = finder.Replace("[", @"^\[").Replace("]", @"^\]\");
            string regString = $"{filler + newFinder}";
            if (checkForEmptySpace) regString = $"\\b" + regString + "\\b";
            baseString = Regex.Replace(baseString, regString, MakeString(PluginConstants.forbiddenCharacter, count), RegexOptions.IgnoreCase);
        }
    }

    public string GetInitials(string value)
    {
        string[] nameParts = value.Split(' ');
        string endString = string.Empty;
        foreach(string str in nameParts)
        {
            if (str.Length == 0) continue;
            endString += str.First().ToString().ToUpperInvariant() + ". ";
        }
        return endString;
    }

    public string MakeString(char c, int count) => new string(c, count);

    public unsafe void ReplaceAtkString(AtkTextNode* textNode, ref (string, string)[] validNames, AtkNineGridNode* nineGridNode = null)
    {
        if (validNames.Length == 0) return;
        string? outcomeText = textNode->NodeText.ToString();
        ReplaceString(ref outcomeText, ref validNames);
        textNode->NodeText.SetString(outcomeText);

        if (nineGridNode == null) return;

        textNode->ResizeNodeForCurrentText();
        nineGridNode->AtkResNode.SetWidth((ushort)(textNode->AtkResNode.Width + 18));
    }

    public unsafe void ReplaceAtkString(AtkTextNode* textNode, string baseName, string replaceName, AtkNineGridNode* nineGridNode = null)
    {
        (string, string)[] strs = new (string, string)[] { (baseName, replaceName) };
        ReplaceAtkString(textNode, ref strs, nineGridNode);
    }
}

public static class StringUtilsHelper
{
    public static string MakeTitleCase(this string str) => StringUtils.instance.MakeTitleCase(str);
}