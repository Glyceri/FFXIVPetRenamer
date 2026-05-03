using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;

namespace PetRenamer.PetNicknames.Hooking.Interfaces;

internal interface ITextHook : IDisposable
{
    bool Faulty { get; }
    
    void Setup(uint[] textPos, Func<PetSkeleton, bool> allowedCallback, bool allowColours, bool isSoft = false);
    void Refresh();
}
