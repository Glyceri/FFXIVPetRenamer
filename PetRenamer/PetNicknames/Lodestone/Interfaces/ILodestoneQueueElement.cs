using PetRenamer.PetNicknames.Lodestone.Enums;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;
using System.Threading;

namespace PetRenamer.PetNicknames.Lodestone.Interfaces;

internal interface ILodestoneQueueElement 
{
    LodestoneQueueState CurrentState { get; }
    DateTime ElementStarted { get; }
    CancellationTokenSource CancellationTokenSource { get; }
    CancellationToken CancellationToken { get; }
    IPettableDatabaseEntry Entry { get; }   
    bool Cancelled { get; }
    void Cancel();
}