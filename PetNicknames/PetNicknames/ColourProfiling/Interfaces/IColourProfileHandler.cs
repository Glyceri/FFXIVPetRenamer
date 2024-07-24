using PetRenamer.PetNicknames.Serialization;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ColourProfiling.Interfaces;

internal interface IColourProfileHandler
{
    int GetActiveAsSerialized();
    SerializableColourProfile[] Serialize();

    List<IColourProfile> ColourProfiles { get; }

    void AddColourProfile(IColourProfile colourProfile);
    void RemoveColourProfile(IColourProfile colourProfile);
    void SetActiveProfile(IColourProfile? profile);

    IColourProfile GetActiveProfile();

    void RegisterWindowHandler(in IWindowHandler windowHandler);

    void Refresh();
}
