using PetRenamer.PetNicknames.Windowing.Enums;

namespace PetNicknames.PetNicknames.Windowing.Interfaces;

internal interface IPetMode
{
    PetWindowMode CurrentMode { get; }
}
