using Dalamud.Utility;
using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Generic;
using Una.Drawing;

namespace PetRenamer.PetNicknames.ColourProfiling;

internal class ColourProfile : IColourProfile
{
    public string Name { get; }
    public string Author { get; }

    public List<PetColour> Colours { get; } = new List<PetColour>();

    public ColourProfile(string name, string author, List<PetColour> colours)
    {
        Name = name;
        Author = author;
        Colours = colours;
    }

    public static ColourProfile Create(IColourParseResult result)
    {
        return new ColourProfile(result.ThemeName, result.ThemeAuthor, result.Colours);
    }

    public void Activate()
    {
        for (int i = 0; i < Colours.Count; i++)
        {
            PetColour colour = Colours[i];
            if (colour.Name.IsNullOrWhitespace()) continue;

            Color.AssignByName(colour.Name, colour.Colour);
        }
    }

    public void SetColor(string name, uint color)
    {
        for(int i = 0; i < Colours.Count; i++)
        {
            PetColour colour = Colours[i];
            if (colour.Name != name) continue;

            Colours[i].Colour = color;
            return;
        }

        Colours.Add(new PetColour(name, color));
    }
}
