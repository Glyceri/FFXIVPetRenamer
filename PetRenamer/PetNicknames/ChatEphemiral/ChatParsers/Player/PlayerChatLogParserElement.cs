using Dalamud.Game.Chat;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Player;

internal class PlayerChatLogParserElement : IChatLogPlayerParserElement
{
    private readonly IChatPlayerDatabase PlayerDatabase;
    
    public PlayerChatLogParserElement(IChatPlayerDatabase playerDatabase)
    {
        PlayerDatabase = playerDatabase;
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