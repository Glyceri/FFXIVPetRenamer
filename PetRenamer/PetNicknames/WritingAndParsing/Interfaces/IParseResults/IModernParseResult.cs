using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

internal interface IModernParseResult : IBaseParseResult
{
    ulong         ContentId     { get; }
    PetSkeleton[] SoftSkeletons { get; }
}
