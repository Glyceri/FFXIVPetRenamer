using Dalamud.Game.Text;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;

internal interface IChatMessageParser
{
    void OnChatMessage(uint messageId, XivChatType xivChatType);
}