using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ColourProfiling.Interfaces;

internal interface IColourProfile
{
    string Name { get; }
    string Author { get; }

    List<PetColour> Colours { get; }

    void SetColor(string name, uint color);
    void Activate();
}
