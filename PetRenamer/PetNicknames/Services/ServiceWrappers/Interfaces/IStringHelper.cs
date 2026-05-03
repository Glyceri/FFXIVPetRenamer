using Dalamud.Game.Chat;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using System.Numerics;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IStringHelper
{
    SeString WrapInColor(string petName, Vector3? edgeColor = null, Vector3? textColor= null);
    
    void ReplaceChat(IHandleableChatMessage chatMessage, IPetSheetData? petData, NameType nameType, IPettableUser? user = null);
    void ReplaceChat(IHandleableChatMessage chatMessage, IPettablePet? pettablePet, NameType nameType);
    
    void ReplaceSeString(ref SeString seString, IPetSheetData? petData, NameType nameType, IPettableUser? user = null);
    void ReplaceSeString(ref SeString seString, IPettablePet? pettablePet, NameType nameType);
    
    void ReplaceATKString(AtkTextNode* atkNode, IPetSheetData? petData, NameType nameType, IPettableUser? user = null);
    void ReplaceATKString(AtkTextNode* atkNode, IPettablePet? pettablePet, NameType nameType);
    
    string CleanupString(string str);
    string CleanupActionName(string str);
    
    Vector3? ParseVector3(string? line);
    string ToVector3String(Vector3 vector);
}
