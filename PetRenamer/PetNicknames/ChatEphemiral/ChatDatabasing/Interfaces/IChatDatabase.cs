using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;

internal interface IChatDatabase<T>
{
    List<T> Elements { get; }
    
    void Clear()
    {
        Elements.Clear();
    }
}