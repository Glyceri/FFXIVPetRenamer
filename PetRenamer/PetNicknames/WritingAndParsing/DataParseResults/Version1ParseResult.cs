using PetRenamer.PetNicknames.Services.ServiceWrappers.Statics;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Numerics;

namespace PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;

internal class Version1ParseResult : IBaseParseResult
{
    public string UserName  { get; }
    public ushort Homeworld { get; }

    public PetSkeleton[] IDs         { get; } = [];
    public string[]      Names       { get; } = [];
    public Vector3?[]    EdgeColous  { get; } = [];
    public Vector3?[]    TextColours { get; } = [];

    public Version1ParseResult(string userName, ushort homeworld)
    {
        UserName  = userName;
        Homeworld = homeworld;
    }

    public Version1ParseResult(string userName, ushort homeworld, int[] ids, string[] names) 
        : this(userName, homeworld)
    {
        IDs         = PetSkeletonHelper.AsPetSkeletons(ids);
        Names       = names;
        EdgeColous  = new Vector3?[ids.Length];
        TextColours = new Vector3?[ids.Length];
    }
}
