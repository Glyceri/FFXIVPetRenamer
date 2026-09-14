using Dalamud.Game.Chat;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Player;

internal class PlayerChatLogParserElement : IChatLogPlayerParserElement
{
    private readonly IPetServices         PetServices;
    private readonly IChatPlayerDatabase  PlayerDatabase;
    
    public PlayerChatLogParserElement(IPetServices petServices, IChatPlayerDatabase playerDatabase)
    {
        PlayerDatabase = playerDatabase;
        PetServices    = petServices;
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
    
    public IChatPlayer? MakeFromLocalPlayer()
    {
        if (PetServices.UserList.LocalPlayer == null)
        {
            return null;
        }
        
        string playerName = PetServices.UserList.LocalPlayer.DataBaseEntry.Name;
        ushort homeworld  = PetServices.UserList.LocalPlayer.DataBaseEntry.Homeworld;
        
        IChatPlayer player = PlayerDatabase.MakeChatPlayer(playerName, homeworld);

        player.MakeStrong(PetServices.UserList.LocalPlayer.DataBaseEntry.ContentId);
        
        return player;
    }
}