using PetRenamer.PetNicknames.Windowing.Enums;
using System;

namespace PetRenamer.PetNicknames.Windowing.Interfaces;

internal interface IPetWindow : IDisposable
{
    void Open();
    void Close();
    void Toggle();

    void SetPetMode(PetWindowMode mode);
    void NotifyDirty();
    bool RequestsModeChange { get; }
    PetWindowMode NewMode { get; }
    void DeclareModeChangedSeen();
}
