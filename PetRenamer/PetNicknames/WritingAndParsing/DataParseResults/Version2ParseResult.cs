﻿using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Numerics;

namespace PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;

internal class Version2ParseResult : IModernParseResult
{
    public string UserName { get; init; }
    public ushort Homeworld { get; init; }
    public ulong ContentID { get; init; }
    public int[] SoftSkeletons { get; init; }

    public int[] IDs { get; init; }
    public string[] Names { get; init; }
    public Vector3?[] EdgeColous { get; }
    public Vector3?[] TextColours { get; }

    public Version2ParseResult(string username, ushort homeworld, ulong contentID, int[] softSkeletons, int[] ids, string[] names)
    {
        UserName = username;
        Homeworld = homeworld;
        ContentID = contentID;
        SoftSkeletons = softSkeletons;
        IDs = ids;
        Names = names;
        EdgeColous = new Vector3?[ids.Length];
        TextColours = new Vector3?[ids.Length];
    }
}
