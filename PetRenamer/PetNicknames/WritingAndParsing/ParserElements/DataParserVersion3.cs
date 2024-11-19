using Dalamud.Utility;
using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.WritingAndParsing.ParserElements;

internal class DataParserVersion3 : IDataParserElement
{
    public IDataParseResult Parse(string data)
    {
        string[] splitLines = data.Split('\n');
        if (splitLines.Length < 5) return new InvalidParseResult("Splitlines was not of length < 5");

        string userName = splitLines[1];
        if (!ushort.TryParse(splitLines[2], out ushort homeWorld))
        {
            return new InvalidParseResult("Homeworld parse failed");
        }

        if (!ulong.TryParse(splitLines[3], out ulong contentID))
        {
            return new InvalidParseResult("ContentID parse failed");
        }

        int[]? softSkeletonsArray = ParseSoftSkeletons(splitLines[4]);
        if (softSkeletonsArray == null)
        {
            return new InvalidParseResult("Soft Skeletons Array parse failed");
        }

        List<int> ids = new List<int>();
        List<string> names = new List<string>();
        List<Vector3?> edgeColour = new List<Vector3?>();
        List<Vector3?> textColour = new List<Vector3?>();

        for (int i = 5; i < splitLines.Length; i++)
        {
            // The reason I dont fully invalidate a parse from a missing or weird line is because people might tinker with their names in the file.
            // A missing entry is not so damning compared to missing or invalid required data.
            try
            {
                string[] splitNickname = splitLines[i].Split(PluginConstants.forbiddenCharacter);
                if (splitNickname.Length < 3) continue;
                if (!int.TryParse(splitNickname[0], out int ID)) continue;
                string nickname = splitNickname[1];
                ids.Add(ID);
                names.Add(nickname);
                edgeColour.Add(ParseVector3(splitNickname[2]));
                textColour.Add(ParseVector3(splitNickname[3]));
            }
            catch { continue; }
        }

        if (ids.Count != names.Count)
        {
            return new InvalidParseResult("IDs and Names don't match up");
        }

        return new Version2ParseResult(userName, homeWorld, contentID, softSkeletonsArray, ids.ToArray(), names.ToArray());
    }

    Vector3? ParseVector3(string line)
    {
        if (line.IsNullOrWhitespace()) return null;

        if (line == "null") return null;

        if (!line.StartsWith("<") || !line.EndsWith(">")) return null;

        line = line.Replace("<", string.Empty).Replace(">", string.Empty);

        string[] numbers = line.Split(',');

        if (numbers.Length != 3) return null;

        if (!float.TryParse(numbers[0], out float X)) return null;
        if (!float.TryParse(numbers[1], out float Y)) return null;
        if (!float.TryParse(numbers[2], out float Z)) return null;

        return new Vector3(X, Y, Z);
    }

    int[]? ParseSoftSkeletons(string data)
    {
        data = data.Replace("[", string.Empty).Replace("]", string.Empty);

        string[] splitData = data.Split(",");
        if (splitData.Length != 5)
        {
            return null;
        }

        List<int> softSkeletons = new List<int>();
        foreach(string s in splitData)
        {
            // Soft skeletons are ALWAYS negative!
            if (!s.StartsWith('-'))
            {
                return null;
            }

            if (!int.TryParse(s, out int id))
            {
                return null;
            }
            softSkeletons.Add(id);
        }

        if (softSkeletons.Count != 5)
        {
            return null;
        }

        return softSkeletons.ToArray();
    }

    /* ------------------- Version 2 File -------------------
     * [PetNicknames(2)]                        // Header
     * Firstname Lastname                       // Username
     * 0                                        // Homeworld ID (ushort)
     * 0                                        // Content ID (ulong)
     * [-000,-000,-000,-000,-000]               // Array of soft skeletons. Always 5 in length. Soft skeletons must always start with a minus
     * 0^customName^edgeColour^textColour       / Start of skeleton to name entries (the ^ is a symbol the user cannot use) 
     * 1^customName^edgeColour^textColour
     * 2^customName^edgeColour^textColour
     * ...
     */
}
