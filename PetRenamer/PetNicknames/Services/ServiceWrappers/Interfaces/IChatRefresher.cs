using System;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IChatRefresher : IDisposable
{
    void RefreshChat();
}