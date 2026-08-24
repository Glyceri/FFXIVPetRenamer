using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;

internal interface IChatEntity : IChatObject
{
    uint LastUsedAt { get; set; }
}