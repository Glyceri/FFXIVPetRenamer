using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Numerics;

namespace PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;

internal class Version3ParseResult : IModernParseResult
{
    public string        UserName      { get; }
    public ushort        Homeworld     { get; }
    public ulong         ContentID     { get; }
    public PetSkeleton[] SoftSkeletons { get; }
    public PetSkeleton[] IDs           { get; }

    public string[]      Names         { get; }
    public Vector3?[]    EdgeColous    { get; }
    public Vector3?[]    TextColours   { get; }

    public Version3ParseResult(string username, ushort homeworld, ulong contentID, int[] softSkeletons, int[] ids, string[] names, Vector3?[] edgeColours, Vector3?[] textColours)
    {
        UserName      = username;
        Homeworld     = homeworld;
        ContentID     = contentID;
        SoftSkeletons = PetSkeletonHelper.AsPetSkeletons(softSkeletons);
        IDs           = PetSkeletonHelper.AsPetSkeletons(ids);
        Names         = names;
        EdgeColous    = edgeColours;
        TextColours   = textColours;
    }
}
