using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using System;

namespace PetRenamer.PetNicknames.Windowing.Interfaces;

internal interface IPetWindow : IDisposable
{
    SkeletonType PetMode { get; }
    
    void Open();
    void Close();
    void Toggle();

    void SetPetMode(SkeletonType mode);
    void NotifyDirty();
    
    bool ShowQuickButtons { get; }
    bool HasModeToggle    { get; }
}
