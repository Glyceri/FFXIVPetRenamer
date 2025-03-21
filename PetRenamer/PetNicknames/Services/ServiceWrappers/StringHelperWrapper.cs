using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using LSeStringBuilder = Lumina.Text.SeStringBuilder;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

// I am probably breaking around 10000000 SeString rules or w/e O.O
// I have no idea, but throughout all my testing it has honestly worked... just fine c:
internal class StringHelperWrapper : IStringHelper
{
    readonly IPetServices PetServices;

    public StringHelperWrapper(IPetServices petServices) 
    { 
        PetServices = petServices;
    }

    public string ToTitleCase(string str) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());

    public SeString WrapInColor(string text, Vector3? edgeColor = null, Vector3? textColor = null)
    {
        if (text.IsNullOrWhitespace()) return SeString.Empty;

        LSeStringBuilder builder = new LSeStringBuilder();

        if (textColor != null) builder.PushColorRgba(new Vector4(textColor.Value, 1f));
        if (edgeColor != null) builder.PushEdgeColorRgba(new Vector4(edgeColor.Value, 1f));
        builder.Append(text);
        if (edgeColor != null) builder.PopEdgeColor();
        if (textColor != null) builder.PopColor();

        return builder.ToReadOnlySeString().ToDalamudString();
    }

    public unsafe string ReplaceATKString(AtkTextNode* atkNode, string baseString, string replaceString, Vector3? edgeColor, Vector3? textColor, IPetSheetData petData, bool checkForEmptySpace = true)
    {
        if (atkNode == null) return baseString;
        SeString newString = ReplaceStringPart(baseString, replaceString, petData, checkForEmptySpace, edgeColor, textColor);
        atkNode->SetText(newString.EncodeWithNullTerminator());
        return newString.TextValue;
    }

    public void ReplaceSeString(ref SeString message, string replaceString, IPetSheetData petData, bool checkForEmptySpace = true, Vector3? edgeColor = null, Vector3? textColor = null)
    {
        if (message == null || message.Payloads.Count == 0) return;

        List<Payload> newPayloads = new List<Payload>();

        for (int i = 0; i < message.Payloads.Count; i++)
        {
            if (message.Payloads[i] is not TextPayload tPayload)
            {
                newPayloads.Add(message.Payloads[i]);
                continue;
            }

            string curString = tPayload.Text!.ToString();
            newPayloads.AddRange(ReplaceStringPart(curString, replaceString, petData, checkForEmptySpace, edgeColor, textColor).Payloads);
        }

        message.Payloads.Clear();
        message.Payloads.AddRange(newPayloads);
    }

    public SeString ReplaceStringPart(string baseString, string replaceString, IPetSheetData petData, bool checkForEmptySpaces = true, Vector3? edgeColor = null, Vector3? textColor = null)
    {
        if (replaceString.IsNullOrWhitespace())
        {
            return new SeString(new TextPayload(baseString));
        }

        SeString newSeString = new SeString();
        List<string> parts = GetString(petData);

        int length = parts.Count;
        int smallerCounter = 0;

        for (int i = 0; i < length; i++)
        {
            string part = parts[i];
            if (part == string.Empty) continue;
            smallerCounter++;
            part = part.Replace("[", @"^\[").Replace("]", @"^\]\");
            string regString = part;
            if (checkForEmptySpaces) regString = $"\\b" + regString + "\\b";
            baseString = Regex.Replace(baseString, regString, PluginConstants.forbiddenCharacter.ToString(), RegexOptions.IgnoreCase);
        }

        string[] splitted = Regex.Split(baseString, @$"(\{PluginConstants.forbiddenCharacter}+)");

        foreach (string s in splitted)
        {
            if (s.IsNullOrWhitespace()) continue;
            if (!s.Contains(PluginConstants.forbiddenCharacter))
            {
                newSeString.Append(new TextPayload(s));
                continue;
            }
            else
            {
                newSeString.Append(WrapInColor(replaceString, edgeColor, textColor));
            }
        }

        return newSeString;
    }

    List<string> GetString(IPetSheetData petData)
    {
        // Can be simplified it said... this is cursed
        List<string> parts = [.. petData.Plural, .. petData.Singular];

        parts.Sort((s1, s2) => s1.Length.CompareTo(s2.Length));
        parts.Reverse();

        return parts;
    }

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
