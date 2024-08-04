namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;

internal interface IModernParseResult : IBaseParseResult
{
    ulong ContentID { get; }
    int[] SoftSkeletons { get; }
}
