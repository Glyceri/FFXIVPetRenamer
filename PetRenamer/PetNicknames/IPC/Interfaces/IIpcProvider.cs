using System;

namespace PetRenamer.PetNicknames.IPC.Interfaces;

internal interface IIpcProvider : IDisposable
{
    void Prepare();
    void NotifyDataChanged();
    void ClearCachedData();
}
