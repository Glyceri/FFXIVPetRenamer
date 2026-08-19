using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Enums;

namespace PetRenamer.PetNicknames.WritingAndParsing.Interfaces;

internal interface IDataWriter
{
    string? WriteData(IPettableUser forUser, ParseVersion forVersion = ParseVersion.COUNT - 1);
}