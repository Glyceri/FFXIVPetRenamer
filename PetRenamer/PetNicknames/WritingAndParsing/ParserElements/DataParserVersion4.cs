using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace PetRenamer.PetNicknames.WritingAndParsing.ParserElements;

internal class DataParserVersion4 : IDataParserElement
{
    private readonly IPetServices PetServices;

    public DataParserVersion4(IPetServices petServices)
    {
        PetServices = petServices;
    }

    public IDataParseResult Parse(string data)
    {
        string[] splitLines = data.Split(Environment.NewLine);

        if (splitLines.Length < 5)
        {
            return new InvalidParseResult("Splitlines was not of length < 5");
        }

        string userName = splitLines[1];

        if (!ushort.TryParse(splitLines[2], out ushort homeWorld))
        {
            return new InvalidParseResult("Homeworld parse failed");
        }

        if (!ulong.TryParse(splitLines[3], out ulong contentID))
        {
            return new InvalidParseResult("ContentID parse failed");
        }

        PetSkeleton[]? softSkeletonsArray = ParseSoftSkeletons(splitLines[4]);

        if (softSkeletonsArray == null)
        {
            return new InvalidParseResult("Soft Skeletons Array parse failed");
        }

        List<int>      ids           = [];
        List<int>      skeletonTypes = [];
        List<string>   names         = [];
        List<Vector3?> edgeColours   = [];
        List<Vector3?> textColours   = [];

        for (int i = 5; i < splitLines.Length; i++)
        {
            // The reason I dont fully invalidate a parse from a missing or weird line is because people might tinker with their names in the file.
            // A missing entry is not so damning compared to missing or invalid required data.
            try
            {
                string[] splitNickname = splitLines[i].Split(PluginConstants.forbiddenCharacter);

                if (splitNickname.Length < 4)
                {
                    continue;
                }

                if (!int.TryParse(splitNickname[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int Id))
                {
                    continue;
                }

                if (!int.TryParse(splitNickname[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int SkeletonType))
                {
                    continue;
                }

                string nickname = splitNickname[2];

                ids.Add(Id);
                skeletonTypes.Add(SkeletonType);
                names.Add(nickname);
                edgeColours.Add(PetServices.StringHelper.ParseVector3(splitNickname[3]));
                textColours.Add(PetServices.StringHelper.ParseVector3(splitNickname[4]));
            }
            catch { }
        }

        int idCount           = ids.Count;
        int nameCount         = names.Count;
        int skeletonTypeCount = skeletonTypes.Count;

        if (idCount != nameCount)
        {
            return new InvalidParseResult("IDs and Names don't match up");
        }

        if (idCount != skeletonTypeCount)
        {
            return new InvalidParseResult("IDs and SkeletonTypes don't match up");
        }

        if (nameCount != skeletonTypeCount)
        {
            return new InvalidParseResult("Names and SkeletonTypes don't match up");
        }

        return new Version4ParseResult(userName, homeWorld, contentID, softSkeletonsArray, PetSkeletonHelper.AsPetSkeletons(ids.ToArray(), skeletonTypes.ToArray()), names.ToArray(), edgeColours.ToArray(), textColours.ToArray());
    }
    
    private PetSkeleton? ParsePetSkeleton(string petSkeletonString)
    {
        if (!petSkeletonString.Contains(PluginConstants.forbiddenCharacter))
        {
            return null;
        }

        string[] skeletonsSplit = petSkeletonString.Split(PluginConstants.forbiddenCharacter);

        if (skeletonsSplit.Length != 2)
        {
            return null;
        }

        if (!int.TryParse(skeletonsSplit[0], out int id))
        {
            return null;
        }

        if (!int.TryParse(skeletonsSplit[1], out int skeletonType))
        {
            return null;
        }

        return new PetSkeleton((uint)id, (SkeletonType)skeletonType);
    }

    private PetSkeleton[]? ParseSoftSkeletons(string data)
    {
        data = data.Replace("[", string.Empty).Replace("]", string.Empty);

        string[] splitData = data.Split(",");

        if (splitData.Length != 5)
        {
            return null;
        }

        List<PetSkeleton> softSkeletons = new List<PetSkeleton>();

        foreach (string str in splitData)
        {
            PetSkeleton? petSkeleton = ParsePetSkeleton(str);

            if (petSkeleton == null)
            {
                return null;
            }

            softSkeletons.Add(petSkeleton.Value);
        }

        if (softSkeletons.Count != PluginConstants.BaseSkeletons.Length)
        {
            return null;
        }

        return [.. softSkeletons];
    }

    /* ------------------- Version 2 File -------------------
     * [PetNicknames(2)]                        // Header
     * Firstname Lastname                       // Username
     * 0                                        // Homeworld ID (ushort)
     * 0                                        // Content ID (ulong)
     * [0^0,0^0,0^0,0^0,0^0]                    // Array of soft skeletons. Always 5 in length. Soft skeletons must always start with a minus
     * 0^0^customName^edgeColour^textColour     // Start of skeleton to name entries (the ^ is a symbol the user cannot use) <0.1,0.1,0.1> is the layout of edgecolour and textcolour
     * 1^0^customName^edgeColour^textColour
     * 2^0^customName^edgeColour^textColour
     * ...
     */
}
