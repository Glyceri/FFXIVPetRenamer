using PN.S;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Enums;
using PetRenamer.PetNicknames.WritingAndParsing.Structs;

namespace PetRenamer.PetNicknames.PettableDatabase.Interfaces;

internal interface IPettableDatabase
{
    IPettableDatabaseEntry[] DatabaseEntries { get; }
    
    IPettableDatabaseEntry? GetEntry(string name, ushort homeworld, bool create);
    IPettableDatabaseEntry? GetEntryNoCreate(ulong contentId);
    IPettableDatabaseEntry GetEntry(ulong contentId);
    
    SerializableUserV6[] SerializeDatabase();
    
    void RemoveEntry(IPettableDatabaseEntry entry, ParseSource parseSource);
    void SetDirty();
    void ApplyParseResult(IModernParseResult parseResult, ParseContext parseContext);
}
