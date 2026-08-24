using Dalamud.Game.Chat;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Player;

internal class PlayerChatLogParserElement : IChatLogPlayerParserElement
{
    private readonly IChatElementDatabase ChatDatabase;
    private readonly IChatPlayerDatabase  PlayerDatabase;
    
    public PlayerChatLogParserElement(IChatPlayerDatabase playerDatabase, IChatElementDatabase chatElementDatabase)
    {
        PlayerDatabase = playerDatabase;
        ChatDatabase   = chatElementDatabase;
    }
    
    public IChatPlayer? Parse(ILogMessageEntity? logMessageEntity)
    {
        if (logMessageEntity == null)
        {
            return null;
        }
        
        if (!logMessageEntity.IsPlayer)
        {
            return null;
        }
        
        string playerName = logMessageEntity.Name.ExtractText();
        ushort homeworld  = logMessageEntity.HomeWorldId;
        
        return PlayerDatabase.MakeChatPlayer(playerName, homeworld);
    }
}