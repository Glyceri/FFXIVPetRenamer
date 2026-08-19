using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Enums;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

internal interface IDataWriterElement
{
    ParseVersion WriteVersion { get; }
    string WriteData(IPettableUser forUser);
}
