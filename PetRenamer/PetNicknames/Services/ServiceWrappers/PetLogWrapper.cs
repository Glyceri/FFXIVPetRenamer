using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Runtime.CompilerServices;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PetLogWrapper : IPetLog
{
    // For debug purposes
    public static IPetLog? Instance;

    private readonly IPluginLog PluginLog;

    public PetLogWrapper(IPluginLog pluginLog) 
    {
        Instance  = this;

        PluginLog = pluginLog;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Log(object? message)
        => PluginLog.Debug($"{message}");
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogError(Exception e, object? message)
        => PluginLog.Error($"{e} : {message}");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogException(Exception e)
        => PluginLog.Error($"{e}");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogFatal(object? message)
        => PluginLog.Fatal($"{message}");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogInfo(object? message)
        => PluginLog.Info($"{message}");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogVerbose(object? message)
        => PluginLog.Verbose($"{message}");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogWarning(object? message)
        => PluginLog.Warning($"{message}");
}
