using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.System.String;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.ChatEphemiral.Interfaces;

internal unsafe interface IEphemaralChatHandler : IEnablableHandler
{
    void OnChatLog(uint messageId, XivChatType xivChatType, uint logMessageId, ILogMessageEntity? sourceEntity, ILogMessageEntity? targetEntity);
    void OnChatMessage(uint messageId, XivChatType xivChatType);
    byte[]? Replace(Utf8String* message, int index);
    void OnChatClear();
}