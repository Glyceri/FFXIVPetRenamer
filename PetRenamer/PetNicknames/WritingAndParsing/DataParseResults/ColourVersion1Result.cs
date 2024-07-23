using PetRenamer.PetNicknames.ColourProfiling;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;

internal class ColourVersion1Result : IColourParseResult
{
    public string ThemeName { get; }
    public string ThemeAuthor { get; }
    public List<PetColour> Colours { get; }

    public ColourVersion1Result(string themeName, string themeAuthor, List<PetColour> colours)
    {
        ThemeName = themeName;
        ThemeAuthor = themeAuthor;
        Colours = colours;
    }
}
