using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Numerics;

namespace PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;

internal class Version4ParseResult : IModernParseResult
{
    public string        UserName      { get; }
    public ushort        Homeworld     { get; }
    public ulong         ContentID     { get; }
    public PetSkeleton[] SoftSkeletons { get; }
    public PetSkeleton[] IDs           { get; }

    public string[]      Names         { get; }
    public Vector3?[]    EdgeColous    { get; }
    public Vector3?[]    TextColours   { get; }

    public Version4ParseResult(string username, ushort homeworld, ulong contentID, PetSkeleton[] softSkeletons, PetSkeleton[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours)
    {
        UserName      = username;
        Homeworld     = homeworld;
        ContentID     = contentID;
        SoftSkeletons = softSkeletons;
        IDs           = ids;
        Names         = names;
        EdgeColous    = edgeColours;
        TextColours   = textColours;
    }
}
