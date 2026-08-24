using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;

internal interface IChatDatabase<T>
    where T : IChatObject
{
    List<T> Elements { get; }
    
    void Clear()
    {
        Elements.Clear();
    }
    
    int Length()
    {
        return Elements.Count;
    }
}