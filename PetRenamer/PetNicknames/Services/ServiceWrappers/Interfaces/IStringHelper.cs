using Dalamud.Game.Chat;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Numerics;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IStringHelper
{
    SeString WrapInColor(string petName, Vector3? edgeColor = null, Vector3? textColor = null);
    
    bool ReplaceChat(Configuration.ColourConfig colourConfig, IHandleableChatMessage chatMessage, IPetSheetData? petData, NameType nameType, IPettableUser? user = null);
    bool ReplaceChat(Configuration.ColourConfig colourConfig, IHandleableChatMessage chatMessage, IPettablePet? pettablePet, NameType nameType);
    
    bool ReplaceAtkString(Configuration.ColourConfig colourConfig, AtkTextNode* atkNode, IPetSheetData? petData, NameType nameType, IPettableUser? user = null);
    bool ReplaceAtkString(Configuration.ColourConfig colourConfig, AtkTextNode* atkNode, IPettablePet? pettablePet, NameType nameType);
    
    bool ReplaceSeString(Configuration.ColourConfig colourConfig, ref SeString seString, PetSkeleton petSkeleton, string baseName, IPettableUser? user = null);
    
    string CleanupActionString(string str);
    
    Vector3? ParseVector3(string? line);
    string ToVector3String(Vector3 vector);
}
