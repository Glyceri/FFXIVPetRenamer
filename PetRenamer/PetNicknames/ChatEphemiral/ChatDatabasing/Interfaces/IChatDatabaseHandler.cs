using System;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;

internal interface IChatDatabaseHandler : IDisposable
{
    IChatPetDatabase     PetDatabase         { get; }
    IChatPlayerDatabase  PlayerDatabase      { get; }
    IChatElementDatabase ChatElementDatabase { get; }
    
    void Clear();
}