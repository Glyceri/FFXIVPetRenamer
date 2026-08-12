using PetRenamer.PetNicknames.ChatEphemiral.Enums;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;

internal interface IChatPlayer : IChatEntity
{
    MessageStrength Strength   { get; }
    string          PlayerName { get; }
    ushort          Homeworld  { get; }
    ulong           ContentId  { get; }
    
    void MakeStrong(ulong contentId);
    void UpdatePlayerData(string playerName, ushort homeworld);
}