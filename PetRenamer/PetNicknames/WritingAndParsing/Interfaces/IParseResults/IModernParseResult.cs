using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

internal interface IModernParseResult : IBaseParseResult
{
    public ulong         ContentID     { get; }
    public PetSkeleton[] SoftSkeletons { get; }
}
