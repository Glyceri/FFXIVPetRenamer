using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using LSeStringBuilder = Lumina.Text.SeStringBuilder;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

// I am probably breaking around 10000000 SeString rules or w/e O.O
// I have no idea, but throughout all my testing it has honestly worked... just fine c:
internal class StringHelperWrapper : IStringHelper
{
    private readonly IPetServices      PetServices;
    private readonly IPettableUserList UserList;

    public StringHelperWrapper(IPetServices petServices, IPettableUserList pettableUserList) 
    { 
        PetServices = petServices;
        UserList    = pettableUserList;
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
        
        // Failsafe... im quite sure the current implementation is a tad bugged :/
        if (PetServices.Configuration.showColours == 2)
        {
            edgeColor = null;
            textColor = null;
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TextPayload CreateNewTextPayload(string text) 
        => new TextPayload(text);
    
    private List<Payload> CreatePayloadsFromReplace(string? baseString, string toReplace, string replaceWith, Vector3? edgeColor, Vector3? textColor)
    {
        List<Payload> newPayloads = [];
        
        if (baseString.IsNullOrWhitespace())
        {
            return newPayloads;
        }
        
        PetServices.PetLog.LogVerbose($"Trying to replace: ['{toReplace}'] with ['{replaceWith}' {edgeColor} {textColor}] in ['{baseString}'].");
        
        string nodeText = baseString;
        
        string regString = toReplace.Replace("[", @"^\[").Replace("]", @"^\]\");
        
        regString = $"\\b" + regString + "\\b";
        
        nodeText = Regex.Replace(nodeText, regString, PluginConstants.forbiddenCharacter.ToString(), RegexOptions.IgnoreCase);
        
        string[] splitStrings = Regex.Split(nodeText, @$"(\{PluginConstants.forbiddenCharacter}+)");
        
        foreach (string splitString in splitStrings)
        {
            if (splitString.IsNullOrWhitespace())
            {
                continue;
            }
            
            if (splitString != PluginConstants.forbiddenCharacter.ToString())
            {
                newPayloads.Add(CreateNewTextPayload(splitString));
            }
            else
            {
                newPayloads.AddRange(WrapInColor(replaceWith, edgeColor, textColor).Payloads);
            }
        }
        
        return newPayloads;
    }
    
    public unsafe void ReplaceATKString(AtkTextNode* atkNode, IPetSheetData? petData, NameType nameType, IPettableUser? user = null)
    {
        if (atkNode == null)
        {
            return;
        }
        
        if (petData == null)
        {
            return;
        }
        
        user ??= UserList.LocalPlayer;
        
        if (user == null)
        {
            return;
        }
        
        string? baseName = PetServices.NameService.GetName(nameType, petData);
        
        if (baseName.IsNullOrWhitespace())
        {
            return;
        }
        
        string? customName = user.GetCustomName(petData);
        
        if (customName.IsNullOrWhitespace())
        {
            return;
        }
        
        user.GetDrawColours(petData, out Vector3? edgeColour, out Vector3? textColour);
        
        ReplaceATKString(atkNode, baseName, customName, edgeColour, textColour);
    }
    
    public unsafe void ReplaceATKString(AtkTextNode* atkNode, IPettablePet? pettablePet, NameType nameType)
    {
        if (atkNode == null)
        {
            return;
        }
        
        if (pettablePet == null)
        {
            return;
        }
        
        IPettableUser? owner    = pettablePet.Owner;
        IPetSheetData? petData  = pettablePet.PetData;
        
        if (owner == null || petData == null)
        {
            return;
        }
        
        string? baseName = PetServices.NameService.GetName(nameType, petData);
        
        if (baseName.IsNullOrWhitespace())
        {
            return;
        }
        
        string? customName = pettablePet.CustomName;
        
        if (customName.IsNullOrWhitespace())
        {
            return;
        }
        
        pettablePet.GetDrawColours(out Vector3? edgeColour, out Vector3? textColour);
        
        ReplaceATKString(atkNode, baseName, customName, edgeColour, textColour);
    }
    
    public unsafe void ReplaceATKString(AtkTextNode* atkNode, string baseString, string replaceString, Vector3? edgeColor, Vector3? textColor)
    {
        if (atkNode == null)
        {
            return;
        }
        
        SeString seString = atkNode->NodeText.AsDalamudSeString();
        
        List<Payload> payloads = seString.Payloads;
        
        for (int i = 0; i < payloads.Count; i++)
        {
            Payload payload = payloads[i];
            
            if (payload.Type != PayloadType.RawText)
            {
                continue;
            }
            
            if (payload is not TextPayload textPayload)
            {
                continue;
            }

            List<Payload> newPayloads = CreatePayloadsFromReplace(textPayload.Text, baseString, replaceString, edgeColor, textColor);
            
            payloads.RemoveAt(i);
            
            payloads.InsertRange(i, newPayloads);
        }
        
        SeString finalSeString = new SeString(payloads);
        
        atkNode->SetText(finalSeString.EncodeWithNullTerminator());
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

            newPayloads.AddRange(ReplaceStringPart(curString, replaceString, edgeColor, textColor, checkForEmptySpace).Payloads);
        }

        message.Payloads.Clear();
        message.Payloads.AddRange(newPayloads);
    }
    
    public SeString ReplaceStringPart(string baseString, string replaceString, Vector3? edgeColor = null, Vector3? textColor = null, bool checkForEmptySpaces = true)
    {
        if (replaceString.IsNullOrWhitespace())
        {
            return new SeString(new TextPayload(baseString));
        }

        SeString newSeString = new SeString();
        
        string regString = replaceString.Replace("[", @"^\[").Replace("]", @"^\]\");

        PetServices.PetLog.LogVerbose(regString);
        
        if (checkForEmptySpaces)
        {
            regString = $"\\b" + regString + "\\b";
        }
        
        baseString = Regex.Replace(baseString, regString, PluginConstants.forbiddenCharacter.ToString(), RegexOptions.IgnoreCase);
        
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
