using System;
using System.Runtime.CompilerServices;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetLog
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Log(object? message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogInfo(object? obj);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogWarning(object? obj);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogFatal(object? obj);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogVerbose(object? obj);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogError(Exception e, object? obj);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogException(Exception e);
}
