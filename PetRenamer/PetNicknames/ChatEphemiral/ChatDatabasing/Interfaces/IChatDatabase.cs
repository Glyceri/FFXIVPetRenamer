using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;

internal interface IChatDatabase<T>
{
    // TODO: Make the elements have a max size
    // For the player and pet list, remove the least used entries,
    // For the ChatElements list, remove the oldest log messages.
    
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