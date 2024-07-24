using PetRenamer.PetNicknames.ColourProfiling;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

internal interface IColourParseResult : IDataParseResult
{
    string ThemeName { get; }
    string ThemeAuthor { get; }

    List<PetColour> Colours { get; }
}
