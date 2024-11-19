using System.ComponentModel;

namespace PetRenamer.PetNicknames.ReadingAndParsing.Enums;

internal enum ParseVersion
{
    Invalid,
    [Description("[PetExport]")]
    Version1,
    [Description("[PetNicknames(2)]")]
    Version2,
    [Description("[PetNicknames(3)]")]
    Version3,
    COUNT
}
