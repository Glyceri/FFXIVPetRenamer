using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatLogParsers.Interfaces;

internal interface IChatLogChatParser
{
    NameType       ReplaceNameType { get; }
    IPetSheetData? ReplaceData     { get; }
    
    void Parse(ILogMessageEntity? sourceEntity, ILogMessageEntity? targetEntity, XivChatType chatType, uint messageId, 
        out IChatPlayer? sourcePlayer, out IChatPlayer? targetPlayer, out IChatPet? sourcePet, out IChatPet? targetPet);
}