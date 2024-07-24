using Newtonsoft.Json;
using System;

namespace PetRenamer.PetNicknames.Serialization;

[Serializable]
public class SerializableColourProfile
{
    public readonly string Name;
    public readonly string Author;

    public readonly string[] ColourNames;
    public readonly uint[] ColourValues;

    [JsonConstructor]
    public SerializableColourProfile(string name, string author, string[] colourNames, uint[] colourValues)
    {
        Name = name;
        Author = author;

        if (colourNames.Length != colourValues.Length)
        {
            ColourNames = [];
            ColourValues = [];
            return;
        }

        ColourNames = colourNames;
        ColourValues = colourValues;
    }
}
