using PetRenamer.PetNicknames.Lodestone.Enums;
using PetRenamer.PetNicknames.Lodestone.Interfaces;
using PetRenamer.PetNicknames.Lodestone.Structs;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using System;
using System.Threading;

namespace PetRenamer.PetNicknames.Lodestone.Lodestone;

internal class LodestoneQueueElement : ILodestoneQueueElement, IDisposable
{
    public LodestoneQueueState CurrentState 
        { get; protected set; } = LodestoneQueueState.Cooking;

    public DateTime ElementStarted 
        { get; } = DateTime.Now;

    public CancellationTokenSource CancellationTokenSource 
        { get; } = new CancellationTokenSource();

    public IPettableDatabaseEntry Entry 
        { get; private set; }

    private readonly PetServices PetServices;

    public readonly Action<IPettableDatabaseEntry, LodestoneSearchData>? Success;
    public readonly Action<Exception>? Failure;

    public LodestoneQueueElement(PetServices petServices, IPettableDatabaseEntry entry, Action<IPettableDatabaseEntry, LodestoneSearchData> success, Action<Exception> failure)
    {
        PetServices = petServices;
        Entry       = entry;
        Success     = success;
        Failure     = failure;
    }

    public CancellationToken CancellationToken 
        => CancellationTokenSource.Token;

    public bool Cancelled 
        => CancellationToken.IsCancellationRequested;

    public void Cancel()
    {
        try
        {
            CancellationTokenSource.Cancel();
        }
        catch(Exception e) 
        {
            PetServices.PetLog.LogException(e);
        }
    }
    
    public void Dispose() 
    {
        Cancel();

        CancellationTokenSource?.Dispose();
        CurrentState = LodestoneQueueState.Disposed;
    }

    public void SetState(LodestoneQueueState state)
    {
        if (CurrentState == LodestoneQueueState.Disposed)
        {
            return;
        }

        CurrentState = state;
    }
}
