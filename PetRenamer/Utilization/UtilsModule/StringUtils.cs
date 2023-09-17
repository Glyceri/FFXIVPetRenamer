using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.Core;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class StringUtils : UtilsRegistryType, ISingletonBase<StringUtils>
{
    public static StringUtils instance { get; set; } = null!;
    public string MakeTitleCase(string str) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLower());

    public void ReplaceSeString(ref SeString message, (string, string)[] validNames)
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

    public PlayerPayload GetPlayerPayload(ref SeString SeString)
    {
        foreach (Payload payload in SeString.Payloads)
            PluginLog.Log(payload.GetType().Name + " : " + payload.ToString());
        //if (payload is PlayerPayload pPayload)
        //return pPayload;
        return null!;
    }

    public unsafe void ReplaceAtkString(AtkTextNode* textNode, (string, string)[] validNames)
    {
        if (validNames.Length == 0) return;
        foreach ((string, string) element in validNames)
        {
            if (element.Item1 == string.Empty || element.Item2 == string.Empty) continue;
            string baseString = textNode->NodeText.ToString();
            textNode->NodeText.SetString(Regex.Replace(baseString, element.Item1, element.Item2, RegexOptions.IgnoreCase));
        }
    }

    public (string, string)[] GetEmoteStrings(string petname, string replaceName)
    {
        List<(string, string)> validNames = new List<(string, string)>();
        if (petname == null) return validNames.ToArray();
        if (petname == string.Empty) return validNames.ToArray();
        foreach (string prefix in PluginConstants.removeables)
            validNames.Add((prefix + petname, replaceName));
        return validNames.ToArray();
    }

    public (string, string)[] GetValidNames(PettableUser user, string beContainedIn)
    {
        List<(string, string)> validNames = new List<(string, string)>();
        if (beContainedIn == null) return validNames.ToArray();
        if (user == null) return validNames.ToArray();
        if (!user.UserExists) return validNames.ToArray();
        foreach (int skelID in RemapUtils.instance.battlePetRemap.Keys)
        {
            string bPetname = SheetUtils.instance.GetBattlePetName(skelID) ?? string.Empty;
            if (bPetname == string.Empty) continue;
            if (!beContainedIn.ToString().Contains(bPetname)) continue;
            if (!RemapUtils.instance.skeletonToClass.ContainsKey(skelID)) continue;
            int jobID = RemapUtils.instance.skeletonToClass[skelID];
            string cName = user.SerializableUser.GetNameFor(jobID) ?? string.Empty;
            if (cName == string.Empty || cName == null) continue;
            validNames.Add((bPetname, cName));
        }
        validNames.Sort((el1, el2) => el1.Item1.Length.CompareTo(el2.Item1.Length));
        return validNames.ToArray();
    }
}
