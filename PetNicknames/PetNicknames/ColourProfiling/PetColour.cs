using PetRenamer.PetNicknames.ColourProfiling.Interfaces;

namespace PetRenamer.PetNicknames.ColourProfiling;

internal class PetColour : IPetColour
{
    public string Name { get; set; }
    public uint Colour { get; set; }

    public PetColour(string name, uint colour)
    {
        Name = name;
        Colour = colour;
    }
}
