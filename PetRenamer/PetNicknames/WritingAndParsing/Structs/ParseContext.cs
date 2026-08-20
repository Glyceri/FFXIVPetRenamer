using PetRenamer.PetNicknames.WritingAndParsing.Enums;

namespace PetRenamer.PetNicknames.WritingAndParsing.Structs;

internal readonly struct ParseContext
{  
    public readonly ParseSource ParseSource;
    public readonly string?     FromPlugin;
    
    public ParseContext(ParseSource parseSource, string? fromPlugin = null)
    {
        ParseSource = parseSource;
        FromPlugin  = fromPlugin;
    }
    
    public readonly bool IsFromIpc
        => ParseSource == ParseSource.IPC;
    
    public override string ToString()
        => $"ParseContext: [{ParseSource}, {FromPlugin}]";
}