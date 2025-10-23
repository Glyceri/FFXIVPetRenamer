using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using LSeStringBuilder = Lumina.Text.SeStringBuilder;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

// I am probably breaking around 10000000 SeString rules or w/e O.O
// I have no idea, but throughout all my testing it has honestly worked... just fine c:
internal class StringHelperWrapper : IStringHelper
{
    private readonly IPetServices PetServices;

    public StringHelperWrapper(IPetServices petServices) 
    { 
        PetServices = petServices;
    }

    private bool GetFloat(string? stringValue, [NotNullWhen(true)] out float value)
        => float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

    public bool TryParseVector3(string? line, [NotNullWhen(true)] out Vector3? vector3)
    {
        vector3 = ParseVector3(line);

        return (vector3 != null);
    }

    public Vector3? ParseVector3(string? line)
    {
        if (line.IsNullOrWhitespace())
        {
            return null;
        }

        if (line == "null")
        {
            return null;
        }

        if (!line.StartsWith('<') && !line.EndsWith('>'))
        {
            return null;
        }

        line = line.CleanString(["<", ">"]);

        string[] numbers = line.Split(',');

        if (numbers.Length != 3)
        {
            return null;
        }

        if (!GetFloat(numbers[0], out float X))
        {
            return null;
        }

        if (!GetFloat(numbers[1], out float Y))
        {
            return null;
        }

        if (!GetFloat(numbers[2], out float Z))
        {
            return null;
        }

        return new Vector3(X, Y, Z);
    }

    public SeString WrapInColor(string text, Vector3? edgeColor = null, Vector3? textColor = null)
    {
        if (text.IsNullOrWhitespace())
        {
            return SeString.Empty;
        }

        LSeStringBuilder builder = new LSeStringBuilder();

        if (textColor != null)
        {
            _ = builder.PushColorRgba(new Vector4(textColor.Value, 1f));
        }

        if (edgeColor != null)
        {
            _ = builder.PushEdgeColorRgba(new Vector4(edgeColor.Value, 1f));
        }

        _ = builder.Append(text);

        if (edgeColor != null)
        {
            _ = builder.PopEdgeColor();
        }

        if (textColor != null)
        {
            _ = builder.PopColor();
        }

        return builder.ToReadOnlySeString().ToDalamudString();
    }

    public unsafe string ReplaceATKString(AtkTextNode* atkNode, string baseString, string replaceString, Vector3? edgeColor, Vector3? textColor, IPetSheetData petData, bool checkForEmptySpace = true)
    {
        if (atkNode == null)
        {
            return baseString;
        }

        SeString newString     = ReplaceStringPart(baseString, replaceString, petData, checkForEmptySpace, edgeColor, textColor);

        byte[]   encodedString = newString.EncodeWithNullTerminator();

        atkNode->SetText(encodedString);

        return newString.TextValue;
    }

    public void ReplaceSeString(ref SeString message, string replaceString, IPetSheetData petData, bool checkForEmptySpace = true, Vector3? edgeColor = null, Vector3? textColor = null)
    {
        if (message == null || message.Payloads.Count == 0)
        {
            return;
        }

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

        SeString     newSeString = new SeString();
        List<string> parts       = GetString(petData);

        int length         = parts.Count;
        int smallerCounter = 0;

        for (int i = 0; i < length; i++)
        {
            string part = parts[i];

            if (part == string.Empty)
            {
                continue;
            }

            smallerCounter++;

            part = part.Replace("[", @"^\[").Replace("]", @"^\]\");

            string regString = part;

            if (checkForEmptySpaces)
            {
                regString = $"\\b" + regString + "\\b";
            }

            baseString = Regex.Replace(baseString, regString, PluginConstants.forbiddenCharacter.ToString(), RegexOptions.IgnoreCase);
        }

        string[] splitted = Regex.Split(baseString, @$"(\{PluginConstants.forbiddenCharacter}+)");

        foreach (string s in splitted)
        {
            if (s.IsNullOrWhitespace())
            {
                continue;
            }

            if (!s.Contains(PluginConstants.forbiddenCharacter))
            {
                _ = newSeString.Append(new TextPayload(s));
            }
            else
            {
                _ = newSeString.Append(WrapInColor(replaceString, edgeColor, textColor));
            }
        }

        return newSeString;
    }

    private List<string> GetString(IPetSheetData petData)
    {
        // Can be simplified it said... this is cursed
        List<string> parts = [.. petData.Plural, .. petData.Singular];

        parts.Sort((s1, s2) =>
        {
            return s1.Length.CompareTo(s2.Length);
        });

        parts.Reverse();

        return parts;
    }

    public string CleanupString(string str)
        => str.CleanString(
            [
                "サモン・", 
                "Summon ", 
                "Invocation ", 
                "-Beschwörung"
            ]);

    public string CleanupActionName(string str)
        => str.CleanString(
            [
                "カーバンクル・", 
                "・エギ", 
                "-Egi", 
                " Carbuncle", 
                "Carbuncle ", 
                "-Karfunkel"
            ]);  
    
    public string ToVector3String(Vector3 vector)
        => vector.ToString("G", CultureInfo.InvariantCulture) ?? "null";
}
