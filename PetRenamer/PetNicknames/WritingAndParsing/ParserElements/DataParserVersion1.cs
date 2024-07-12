using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.WritingAndParsing.ParserElements;

internal class DataParserVersion1 : IDataParserElement
{
    public IDataParseResult Parse(string data)
    {
        string[] splitLines = data.Split('\n');
        if (splitLines.Length < 3) return new InvalidParseResult("Splitlines was not of length < 3");

        string userName = splitLines[1];
        if (!ushort.TryParse(splitLines[2], out ushort homeWorld))
        {
            return new InvalidParseResult("Homeworld parse failed");
        }

        List<int> ids = new List<int>();
        List<string> names = new List<string>();

        for (int i = 3; i < splitLines.Length; i++)
        {
            // The reason I dont fully invalidate a parse from a missing or weird line is because people might tinker with their names in the file.
            // A missing entry is not so damning compared to missing or invalid required data.
            try
            {
                string[] splitNickname = splitLines[i].Split(PluginConstants.forbiddenCharacter);
                if (splitNickname.Length < 1) continue;
                if (!int.TryParse(splitNickname[0].Replace("ID:", ""), out int ID)) continue;
                string nickname = splitNickname[1].Replace("Name:", "");
                ids.Add(ID);
                names.Add(nickname);
            }
            catch { continue; }
        }

        if (ids.Count !=  names.Count)
        {
            return new InvalidParseResult("IDs and Names don't match up");
        }

        return new Version1ParseResult(userName, homeWorld, ids.ToArray(), names.ToArray());
    }

    /* ------------------- Version 1 File -------------------
     * [PetExport]                          // Header
     * Firstname Lastname                   // Username
     * 0                                    // Homeworld ID
     * ID:0^Name:sampleNickname
     * ID:1^Name:sampleNickname
     * ID:2^Name:sampleNickname
     * ...
     */
}
