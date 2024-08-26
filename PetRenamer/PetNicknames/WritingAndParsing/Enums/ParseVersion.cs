using System.ComponentModel;

namespace PetRenamer.PetNicknames.ReadingAndParsing.Enums;

internal enum ParseVersion
{
    Invalid,
    [Description("[PetExport]")]
    Version1,
    [Description("[PetNicknames(2)]")]
    Version2,

    COUNT
}
