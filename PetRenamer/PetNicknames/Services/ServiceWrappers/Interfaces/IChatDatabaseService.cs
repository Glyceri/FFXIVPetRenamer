using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IChatDatabaseService : IDisposable
{
    IChatPetDatabase     PetDatabase         { get; }
    IChatPlayerDatabase  PlayerDatabase      { get; }
    IChatElementDatabase ChatElementDatabase { get; }
    
    void Clear();
}