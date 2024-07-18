using PetRenamer.PetNicknames.Windowing.Enums;
using System;

namespace PetRenamer.PetNicknames.Windowing.Interfaces;

internal interface IPetWindow : IDisposable
{
    void Open();
    void Close();
    void Toggle();

    void Draw();
    void OnDraw();

    void SetPetMode(PetWindowMode mode);
    void OnDirty();
    bool RequestsModeChange { get; }
    PetWindowMode NewMode { get; }
    void DeclareModeChangedSeen();
}
