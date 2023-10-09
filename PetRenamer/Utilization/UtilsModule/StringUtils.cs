using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core;
using PetRenamer.Core.Singleton;
using PetRenamer.Logging;
using PetRenamer.Utilization.Attributes;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class StringUtils : UtilsRegistryType, ISingletonBase<StringUtils>
{
    public static StringUtils instance { get; set; } = null!;
    public string MakeTitleCase(string str) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());

    public PlayerPayload? GetPlayerPayload(ref SeString message)
    {
        if (message == null) return null!;
        for (int i = 0; i < message.Payloads.Count; i++)
        {
            if (message.Payloads[i] is not PlayerPayload pPayload) continue;
            return pPayload;
        }
        return null!;
    }

    public void ReplaceSeString(ref SeString message, ref (string, string)[] validNames)
    {
        if (validNames.Length == 0) return;
        if (message == null) return;
        for (int i = 0; i < message.Payloads.Count; i++)
        {
            if (message.Payloads[i] is not TextPayload tPayload) continue;

            string curString = tPayload.Text!.ToString();
            ReplaceString(ref curString, ref validNames);
            tPayload.Text = curString;

            message.Payloads[i] = tPayload;
        }
    }

    public void ReplaceSeString(ref SeString message, string baseString, string replaceString)
    {
        (string, string)[] strs = new (string, string)[] { (baseString, replaceString) };
        ReplaceSeString(ref message, ref strs);
    }

    public void ReplaceString(ref string baseString, string standard, string replacer)
    {
        (string, string)[] strs = new (string, string)[] { (standard, replacer) };
        ReplaceString(ref baseString, ref strs);
    }

    public void ReplaceString(ref string baseString, ref (string, string)[] validNames)
    {
        int counter = 1;

        foreach ((string, string) str in validNames)
        {
            if (str.Item1 == string.Empty || str.Item2 == string.Empty) continue;
            SanitizeString(ref baseString, str.Item2, ++counter);
            SanitizeString(ref baseString, str.Item1, ++counter);
        }
        for(int i = validNames.Length - 1; i >= 0; i--)
        {
            (string, string) str = validNames[i];
            if (str.Item1 == string.Empty || str.Item2 == string.Empty) continue;
            baseString = baseString.Replace(MakeString(PluginConstants.forbiddenCharacter, counter--), str.Item2);
            baseString = baseString.Replace(MakeString(PluginConstants.forbiddenCharacter, counter--), str.Item2);
        }
    }

    public void SanitizeString(ref string baseString, string finder, int count)
    {
        foreach (string filler in PluginConstants.removeables)
           baseString = Regex.Replace(baseString, @$"\b{filler + finder}\b", MakeString(PluginConstants.forbiddenCharacter, count), RegexOptions.IgnoreCase);
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