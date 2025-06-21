using PetRenamer.PetNicknames.Update.Interfaces;
using System;

namespace PetRenamer.PetNicknames.IPC.Interfaces;

internal interface IIpcProvider : IUpdatable, IDisposable
{
    void Prepare();
    void NotifyDataChanged();
    void ClearCachedData();
}
