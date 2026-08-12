using Dalamud.Game.Chat;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.LanguageBased.Values;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using LSeStringBuilder = Lumina.Text.SeStringBuilder;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class StringHelperWrapper : IStringHelper
{
    private static readonly EmptySpaceValue ReplaceEmptySpaceFor = new EmptySpaceValue()
    {
        EnglishValue = true,
        GermanValue  = true,
        FrenchValue  = true,
    };
    
    private readonly IPetServices    PetServices;
    private readonly IUserList       UserList;
    private readonly DalamudServices DalamudServices;

    public StringHelperWrapper(IPetServices petServices, DalamudServices dalamudServices, IUserList userList) 
    { 
        PetServices     = petServices;
        UserList        = userList;
        DalamudServices = dalamudServices;
    }

    private bool GetFloat(string? stringValue, [NotNullWhen(true)] out float value)
        => float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

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

        if (!GetFloat(numbers[0], out float x) || !GetFloat(numbers[1], out float y) || !GetFloat(numbers[2], out float z))
        {
            return null;
        }
        
        return new Vector3(x, y, z);
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
        
        PetServices.PetLog.DevLogVerbose($"Trying to replace: ['{toReplace}'] with ['{replaceWith}' {edgeColor} {textColor}] in ['{baseString}'].");

        string nodeText = baseString;
        
        string regString = toReplace.Replace("[", @"^\[").Replace("]", @"^\]\");
        
        if (ReplaceEmptySpaceFor.GetValue(DalamudServices))
        {
            regString = $"\\b" + regString + "\\b";
        }
        
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
    
    private unsafe bool MakeSeString(AtkTextNode* atkNode, [NotNullWhen(true)] out SeString? seString)
    {
        seString = null;
        
        if (atkNode == null)
        {
            return false;
        }
        
        if (atkNode->OriginalTextPointer.HasValue)
        {
            // This is so text that gets cut off like Emerald Carbu... gets a chance to replace properly still.
            seString = atkNode->OriginalTextPointer.AsDalamudSeString();
        }
        else
        {
            seString = atkNode->NodeText.AsDalamudSeString();
        }
        
        return true;
    }
    
    public unsafe bool ReplaceAtkString(Configuration.ColourConfig colourConfig, AtkTextNode* atkNode, IPetSheetData? petData, NameType nameType, IPettableUser? user = null)
    {
        if (!MakeSeString(atkNode, out SeString? seString))
        {
            return false;
        }
        
        bool madeReplacement = ReplaceSeString(colourConfig, ref seString, petData, nameType, user);

        atkNode->SetText(seString.EncodeWithNullTerminator());
        
        return madeReplacement;
    }
    
    public unsafe bool ReplaceAtkString(Configuration.ColourConfig colourConfig, AtkTextNode* atkNode, IPettablePet? pettablePet, NameType nameType)
    {
        if (!MakeSeString(atkNode, out SeString? seString))
        {
            return false;
        }
        
        bool madeReplacement = ReplaceSeString(colourConfig, ref seString, pettablePet, nameType);

        atkNode->SetText(seString.EncodeWithNullTerminator());
        
        return madeReplacement;
    }

    public bool ReplaceChat(Configuration.ColourConfig colourConfig, IHandleableChatMessage chatMessage, IPettablePet? pettablePet, NameType nameType)
    {
        SeString seString = chatMessage.Message;
        
        bool madeReplacement = ReplaceSeString(colourConfig, ref seString, pettablePet, nameType);
        
        chatMessage.Message = seString;
        
        return madeReplacement;
    }
    
    public bool ReplaceChat(Configuration.ColourConfig colourConfig, IHandleableChatMessage chatMessage, IPetSheetData? petData, NameType nameType, IPettableUser? user = null)
    {
        SeString seString = chatMessage.Message;
        
        bool madeReplacement = ReplaceSeString(colourConfig, ref seString, petData, nameType, user);
        
        chatMessage.Message = seString;
        
        return madeReplacement;
    }
    
    private bool ReplaceSeString(Configuration.ColourConfig colourConfig, ref SeString seString, IPettablePet? pettablePet, NameType nameType)
    {
        if (pettablePet == null)
        {
            return false;
        }
        
        IPettableUser? owner    = pettablePet.Owner;
        IPetSheetData? petData  = pettablePet.PetData;
        
        return ReplaceSeString(colourConfig, ref seString, petData, nameType, owner);
    }
    
    private bool ReplaceSeString(Configuration.ColourConfig colourConfig, ref SeString seString, IPetSheetData? petData, NameType nameType, IPettableUser? user = null)
    {
        if (petData == null)
        {
            return false;
        }

        string? baseName = PetServices.NameService.GetName(nameType, petData);
        
        if (baseName.IsNullOrWhitespace())
        {
            return false;
        }
        
        return ReplaceSeString(colourConfig, ref seString, petData.Model, baseName, user);
    }
    
    public bool ReplaceSeString(Configuration.ColourConfig colourConfig, ref SeString seString, PetSkeleton petSkeleton, string baseName, IPettableUser? user = null)
    {
        if (!colourConfig.Enabled)
        {
            return false;
        }
        
        user ??= UserList.LocalPlayer;
        
        if (user == null)
        {
            return false;
        }
        
        string? customName = user.GetCustomName(petSkeleton);
        
        if (customName.IsNullOrWhitespace())
        {
            return false;
        }
        
        user.GetDrawColours(petSkeleton, colourConfig, out Vector3? edgeColour, out Vector3? textColour);
        
        return ReplaceSeString(ref seString, baseName, customName, edgeColour, textColour);
    }
    
    private bool ReplaceSeString(ref SeString seString, string baseString, string replaceString, Vector3? edgeColor, Vector3? textColor)
    {
        bool hasMadeReplacement = false;
        
        for (int i = 0; i < seString.Payloads.Count; i++)
        {
            Payload payload = seString.Payloads[i];
            
            if (payload.Type != PayloadType.RawText)
            {
                continue;
            }
            
            if (payload is not TextPayload textPayload)
            {
                continue;
            }
            
            if (textPayload.Text?.Contains(replaceString, StringComparison.InvariantCultureIgnoreCase) ?? true)
            {
                continue;
            }
            
            List<Payload> newPayloads = CreatePayloadsFromReplace(textPayload.Text, baseString, replaceString, edgeColor, textColor);
            
            hasMadeReplacement = (newPayloads.Count > 0);
            
            seString.Payloads.RemoveAt(i);
            
            seString.Payloads.InsertRange(i, newPayloads);
            
            i += newPayloads.Count;
        }
        
        return hasMadeReplacement;
    }

    public string CleanupActionString(string str)
        => str.CleanString(PluginConstants.SummonLanguageValue.GetValue(DalamudServices));
    
    public string ToVector3String(Vector3 vector)
        => vector.ToString("G", CultureInfo.InvariantCulture) ?? "null";
}
