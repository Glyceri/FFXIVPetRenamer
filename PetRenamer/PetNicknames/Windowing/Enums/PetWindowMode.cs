using System.ComponentModel;

namespace PetRenamer.PetNicknames.Windowing.Enums;

internal enum PetWindowMode
{
    [Description("Minion")]
    Minion,
    [Description("Battle Pet")]
    BattlePet,

    //[Description("Beast Master")]
    //BeastMaster,

    COUNT
}
