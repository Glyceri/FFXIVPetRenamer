using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Numerics;

namespace PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;

internal class Version2ParseResult : IModernParseResult
{
    public string UserName { get; }
    public ushort Homeworld { get; }
    public ulong ContentID { get; }
    public PetSkeleton[] SoftSkeletons { get; }

    public PetSkeleton[] IDs { get; init; }
    public string[] Names { get; init; }
    public Vector3?[] EdgeColous { get; }
    public Vector3?[] TextColours { get; }

    public Version2ParseResult(string username, ushort homeworld, ulong contentID, int[] softSkeletons, int[] ids, string[] names)
    {
        UserName = username;
        Homeworld = homeworld;
        ContentID = contentID;
        SoftSkeletons = PetSkeletonHelper.AsPetSkeletons(softSkeletons);
        IDs = PetSkeletonHelper.AsPetSkeletons(ids);
        Names = names;
        EdgeColous = new Vector3?[ids.Length];
        TextColours = new Vector3?[ids.Length];
    }
}
