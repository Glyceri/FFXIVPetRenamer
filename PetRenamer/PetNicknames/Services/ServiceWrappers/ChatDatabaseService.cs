using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class ChatDatabaseService : IChatDatabaseService
{
    public IChatPetDatabase     PetDatabase         { get; }
    public IChatPlayerDatabase  PlayerDatabase      { get; }
    public IChatElementDatabase ChatElementDatabase { get; }
    
    public ChatDatabaseService(IPettableDatabase database, IPetServices petServices)
    {
        PlayerDatabase      = new ChatPlayerDatabase(petServices);
        PetDatabase         = new ChatPetDatabase(petServices);
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