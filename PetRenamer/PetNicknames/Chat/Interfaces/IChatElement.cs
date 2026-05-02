using Dalamud.Game.Chat;

namespace PetRenamer.PetNicknames.Chat.Interfaces;

internal interface IChatElement
{
    void OnChatMessage(IHandleableChatMessage chatMessage);
}
