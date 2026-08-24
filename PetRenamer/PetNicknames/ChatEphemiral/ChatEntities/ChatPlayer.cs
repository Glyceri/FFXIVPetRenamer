using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Enums;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatEntities;

internal class ChatPlayer : IChatPlayer
{
    public MessageStrength Strength   { get; private set; }
    public string          PlayerName { get; private set; }
    public ushort          Homeworld  { get; private set; }
    public ulong           ContentId  { get; private set; }
    public uint            LastUsedAt { get; set; }
    
    public ChatPlayer(string playerName, ushort homeworld)
    {
        Strength   = MessageStrength.Weak;
        ContentId  = 0;
        PlayerName = playerName;
        Homeworld  = homeworld;
    }
  
    public ChatPlayer(ulong contentId, string playerName, ushort homeworld)
        : this(playerName, homeworld)
    {
        Strength  = MessageStrength.Strong;
        ContentId = contentId;
    }
    
    public void MakeStrong(ulong contentId)
    {
        Strength  = MessageStrength.Strong;
        ContentId = contentId;
    }
    
    public void UpdatePlayerData(string playerName, ushort homeworld)
    {
        PlayerName = playerName;
        Homeworld  = homeworld;
    }
}