using PetRenamer.PetNicknames.Serialization;

namespace PetRenamer.PetNicknames.ColourProfiling.Interfaces;

internal interface IColourProfileHandler
{
    void SetActiveProfile(SerializableColourProfile? profile);
    IColourProfile? GetActive();
    SerializableColourProfile? GetActiveProfile();
    SerializableColourProfile[] Serialize();
}
