using PetRenamer.PetNicknames.Lodestone.Enums;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;
using System.Threading;

namespace PetRenamer.PetNicknames.Lodestone.Interfaces;

internal interface ILodestoneQueueElement 
{
    public LodestoneQueueState CurrentState { get; }
    public DateTime ElementStarted { get; }
    public CancellationTokenSource CancellationTokenSource { get; }
    public CancellationToken CancellationToken { get; }
    public IPettableDatabaseEntry Entry { get; }
    public bool Cancelled { get; }
    public void Cancel();
}