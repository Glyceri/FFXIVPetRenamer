using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;

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

    public string MakeTitleCase(string str) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());

    public SeString MakeSeString(string petName, Vector3? edgeColor = null, Vector3? textColor = null)
    {
        if (petName.IsNullOrWhitespace()) return SeString.Empty;

        SeStringBuilder builder = new SeStringBuilder();
        if (textColor != null) builder.Add(new ColorPayload(textColor.Value));
        if (edgeColor != null) builder.Add(new GlowPayload(edgeColor.Value));
        builder.AddText(petName);
        if (edgeColor != null) builder.Add(new GlowEndPayload());
        if (textColor != null) builder.Add(new ColorEndPayload());

        return builder.Build();
    }

    public unsafe string ReplaceATKString(AtkTextNode* atkNode, string baseString, string replaceString, Vector3? edgeColor, Vector3? textColor, IPetSheetData petData, bool checkForEmptySpace = true)
    {
        if (atkNode == null) return baseString;
        SeString newString = ReplaceStringPart(baseString, replaceString, petData, checkForEmptySpace, edgeColor, textColor);
        SetAtkString(atkNode, newString);
        return newString.TextValue;
    }

    unsafe void SetAtkString(AtkTextNode* atkNode, SeString seString)
    {
        atkNode->SetText(seString.Encode());
    }

    public void ReplaceSeString(ref SeString message, string replaceString, IPetSheetData petData, bool checkForEmptySpace = true, Vector3? edgeColor = null, Vector3? textColor = null)
    {
        List<Payload> newPayloads = new List<Payload>();

        if (message == null) return;
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
        List<Payload> newPayloads = new List<Payload>();
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
                newPayloads.Add(new TextPayload(s));
                continue;
            }
            else
            {
                if (edgeColor != null) newPayloads.Add(new GlowPayload(edgeColor.Value));
                if (textColor != null) newPayloads.Add(new ColorPayload(textColor.Value));
                newPayloads.Add(new TextPayload(replaceString));
                if (edgeColor != null) newPayloads.Add(new GlowEndPayload());
                if (textColor != null) newPayloads.Add(new ColorEndPayload());
            }
        }

        return new SeString(newPayloads);
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

public abstract class AbstractColorPayload : Payload
{
    protected byte Red { get; init; }
    protected byte Green { get; init; }
    protected byte Blue { get; init; }

    protected override byte[] EncodeImpl()
    {
        return new byte[] { START_BYTE, ChunkType, 0x05, 0xF6, Red, Green, Blue, END_BYTE };
    }

    protected override void DecodeImpl(BinaryReader reader, long endOfStream)
    {

    }
    public override PayloadType Type => PayloadType.Unknown;

    protected abstract byte ChunkType { get; }

}

public abstract class AbstractColorEndPayload : Payload
{
    protected override byte[] EncodeImpl()
    {
        return new byte[] { START_BYTE, ChunkType, 0x02, 0xEC, END_BYTE };
    }

    protected override void DecodeImpl(BinaryReader reader, long endOfStream)
    {

    }
    public override PayloadType Type => PayloadType.Unknown;

    protected abstract byte ChunkType { get; }
}

public class ColorPayload : AbstractColorPayload
{
    protected override byte ChunkType => 0x13;

    public ColorPayload(Vector3 color)
    {
        Red = Math.Max((byte)1, (byte)(color.X * 255f));
        Green = Math.Max((byte)1, (byte)(color.Y * 255f));
        Blue = Math.Max((byte)1, (byte)(color.Z * 255f));
    }
}

public class ColorEndPayload : AbstractColorEndPayload
{
    protected override byte ChunkType => 0x13;
}

public class GlowPayload : AbstractColorPayload
{
    protected override byte ChunkType => 0x14;

    public GlowPayload(Vector3 color)
    {
        Red = Math.Max((byte)1, (byte)(color.X * 255f));
        Green = Math.Max((byte)1, (byte)(color.Y * 255f));
        Blue = Math.Max((byte)1, (byte)(color.Z * 255f));
    }
}

public class GlowEndPayload : AbstractColorEndPayload
{
    protected override byte ChunkType => 0x14;
}
