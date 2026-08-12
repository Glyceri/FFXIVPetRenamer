using Dalamud.Game.Chat;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;

internal interface IChatLogPlayerParserElement : IChatLogParserElement
{
    IChatPlayer? Parse(ILogMessageEntity? logMessageEntity);
}