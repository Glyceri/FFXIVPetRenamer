using PetRenamer.PetNicknames.Windowing.Enums;

namespace PetRenamer.PetNicknames.Windowing.Interfaces;

internal interface IPetWindow
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
