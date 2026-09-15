using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;

internal interface IChatPlayerDatabase : IChatDatabase<IChatPlayer>
{
    const uint MAX_CHAT_ELEMENTS = 4096;
    const uint CLEANUP_COUNT     = 512;
    
    IChatPlayer? FindChatPlayer(string playerName, ushort homeworld);
    IChatPlayer  MakeChatPlayer(string playerName, ushort homeworld);
}