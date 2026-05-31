using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Interfaces;

internal interface IPetWindow : IDisposable
{
    SkeletonType PetMode { get; }
    
    void Open();
    void Close();
    void Toggle();

    void SetPetMode(SkeletonType mode);
    void NotifyDirty();
    
    bool HasFocus { get; }
    bool HasModeToggle  { get; }
    
    Vector2 CurrentPosition { get; }
}
