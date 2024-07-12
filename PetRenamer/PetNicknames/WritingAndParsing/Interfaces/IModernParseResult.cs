namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

internal interface IModernParseResult : IBaseParseResult
{
    ulong ContentID { get; }
    int[] SoftSkeletons { get; }
}
