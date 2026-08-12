using Dalamud.Game.Chat;
using Dalamud.Game.Text;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;

internal interface IChatLogParser
{
    void OnChatLog(uint messageId, XivChatType xivChatType, uint logMessageId, ILogMessageEntity? sourceEntity, ILogMessageEntity? targetEntity);
}