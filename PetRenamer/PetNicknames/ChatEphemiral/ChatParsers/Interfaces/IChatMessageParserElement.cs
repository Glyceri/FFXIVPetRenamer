using Dalamud.Game.Text;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;

internal interface IChatMessageParserElement
{
    bool IsMyMessage(XivChatType type);
    void Parse(uint messageId, XivChatType type);
}