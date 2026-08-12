using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;

internal interface IChatPlayerDatabase : IChatDatabase<IChatPlayer>
{
    IChatPlayer? FindChatPlayer(string playerName, ushort homeworld);
    IChatPlayer  MakeChatPlayer(string playerName, ushort homeworld);
}