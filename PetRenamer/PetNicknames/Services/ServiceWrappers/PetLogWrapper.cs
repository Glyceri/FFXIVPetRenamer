using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Runtime.CompilerServices;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PetLogWrapper : IPetLog
{
    // For debug purposes
    public static IPetLog? Instance;

    private readonly IPluginLog    PluginLog;
    private readonly Configuration Configuration;

    public PetLogWrapper(IPluginLog pluginLog, Configuration configuration) 
    {
        Instance      = this;
        PluginLog     = pluginLog;
        Configuration = configuration;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DevLog(object? message)
    {
        if (!Configuration.debugModeActive)
        {
            return;
        }

        Log(message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DevLogInfo(object? obj)
    {
        if (!Configuration.debugModeActive)
        {
            return;
        }

        LogInfo(obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DevLogWarning(object? obj)
    {
        if (!Configuration.debugModeActive)
        {
            return;
        }

        LogWarning(obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DevLogFatal(object? obj)
    {
        if (!Configuration.debugModeActive)
        {
            return;
        }

        LogFatal(obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DevLogVerbose(object? obj)
    {
        if (!Configuration.debugModeActive)
        {
            return;
        }

        LogVerbose(obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DevLogError(Exception e, object? obj)
    {
        if (!Configuration.debugModeActive)
        {
            return;
        }

        LogError(e, obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DevLogException(Exception e)
    {
        if (!Configuration.debugModeActive)
        {
            return;
        }

        LogException(e);
    }
}
