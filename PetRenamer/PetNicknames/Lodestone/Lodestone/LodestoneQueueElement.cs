using PetRenamer.PetNicknames.Lodestone.Enums;
using PetRenamer.PetNicknames.Lodestone.Interfaces;
using PetRenamer.PetNicknames.Lodestone.Structs;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;
using System.Threading;

namespace PetRenamer.PetNicknames.Lodestone.Lodestone;

internal class LodestoneQueueElement : ILodestoneQueueElement, IDisposable
{
    public LodestoneQueueState CurrentState { get; protected set; } = LodestoneQueueState.Cooking;
    public DateTime ElementStarted { get; } = DateTime.Now;
    public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
    public CancellationToken CancellationToken { get => CancellationTokenSource.Token; }

    public IPettableDatabaseEntry Entry { get; private set; }
    public bool Cancelled { get => CancellationToken.IsCancellationRequested; }

    public readonly Action<IPettableDatabaseEntry, LodestoneSearchData>? Success;
    public readonly Action<Exception>? Failure;

    public LodestoneQueueElement(in IPettableDatabaseEntry entry, in Action<IPettableDatabaseEntry, LodestoneSearchData> success, in Action<Exception> failure)
    {
        Entry = entry;
        Success = success;
        Failure = failure;
    }

    public void Cancel()
    {
        try
        {
            CancellationTokenSource.Cancel();
        }
        catch { }
    }
    
    public void Dispose() {
        Cancel();
        CancellationTokenSource?.Dispose();
        CurrentState = LodestoneQueueState.Disposed;
    }

    public void SetState(LodestoneQueueState state)
    {
        if (CurrentState == LodestoneQueueState.Disposed) return;
        CurrentState = state;
    }
}
