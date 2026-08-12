using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing;

internal class ChatDatabaseHandler : IChatDatabaseHandler
{
    public IChatPetDatabase     PetDatabase    { get; }
    public IChatPlayerDatabase  PlayerDatabase { get; }
    public IChatElementDatabase ChatElementDatabase { get; }
    
    public ChatDatabaseHandler(IPettableDatabase database, IPetServices petServices)
    {
        PlayerDatabase      = new ChatPlayerDatabase(database);
        PetDatabase         = new ChatPetDatabase(PlayerDatabase);
        ChatElementDatabase = new ChatElementDatabase(petServices);
    }
    
    public void Dispose()
    {
        Clear();
    }
    
    public void Clear()
    {
        PetDatabase.Clear();
        PlayerDatabase.Clear();
        ChatElementDatabase.Clear();
    }
}