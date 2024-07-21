using PetRenamer.PetNicknames.ColourProfiling;
using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.WritingAndParsing.ParserElements;

internal class ColourParserVersion1 : IDataParserElement
{
    public IDataParseResult Parse(string data)
    {
        string[] splitLines = data.Split('\n');
        if (splitLines.Length < 3) return new InvalidParseResult("Splitlines was not of length < 2");

        string themeName = splitLines[1];
        string themeAuthor = splitLines[2];

        List<PetColour> Colours = new List<PetColour>();

        for (int i = 3; i < splitLines.Length; i++)
        {
            try
            {
                string[] splitColour = splitLines[i].Split(PluginConstants.forbiddenCharacter);
                if (splitColour.Length != 2) continue;

                string colourName = splitColour[0];
                uint colour = uint.Parse(splitColour[1]);

                Colours.Add(new PetColour(colourName, colour));
            } 
            catch { }
        }

        return new ColourVersion1Result(themeName, themeAuthor, Colours);
    }
}
