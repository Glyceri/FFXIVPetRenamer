using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class StringUtils : UtilsRegistryType, ISingletonBase<StringUtils>
{
    public static StringUtils instance { get; set; } = null!;
    public string MakeTitleCase(string str) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());

    public void ReplaceSeString(ref SeString message, ref (string, string)[] validNames)
    {
        if (validNames.Length == 0) return;
        if (message == null) return;
        for (int i = 0; i < message.Payloads.Count; i++)
        {
            if (message.Payloads[i] is not TextPayload tPayload) continue;

            foreach ((string, string) element in validNames)
            {
                if (element.Item1 == string.Empty || element.Item2 == string.Empty) continue;
                tPayload.Text = Regex.Replace(tPayload.Text!, element.Item1, element.Item2, RegexOptions.IgnoreCase);
            }
            message.Payloads[i] = tPayload;
        }
    }

    public unsafe void ReplaceAtkString(AtkTextNode* textNode, ref (string, string)[] validNames)
    {
        if (validNames.Length == 0) return;
        foreach ((string, string) element in validNames)
        {
            if (element.Item1 == string.Empty || element.Item2 == string.Empty) continue;
            string baseString = textNode->NodeText.ToString();
            textNode->NodeText.SetString(Regex.Replace(baseString, element.Item1, element.Item2, RegexOptions.IgnoreCase));
        }
    }
}
