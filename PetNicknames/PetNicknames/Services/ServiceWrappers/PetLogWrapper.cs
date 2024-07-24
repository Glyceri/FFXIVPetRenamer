using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PetLogWrapper : IPetLog
{
    readonly IPluginLog PluginLog;

    public PetLogWrapper(IPluginLog pluginLog) => PluginLog = pluginLog;

    public void Log(object? message)
    {
        if (message == null) return;
        PluginLog.Debug($"{message}");
    }

    public void LogError(Exception e, object? message)
    {
        if (message == null) return;
        PluginLog.Error($"{e} : {message}");
    }

    public void LogException(Exception e)
    {
        PluginLog.Error($"{e}");
    }

    public void LogFatal(object? message)
    {
        if (message == null) return;
        PluginLog.Fatal($"{message}");
    }

    public void LogInfo(object? message)
    {
        if (message == null) return;
        PluginLog.Info($"{message}");
    }

    public void LogVerbose(object? message)
    {
        if (message == null) return;
        PluginLog.Verbose($"{message}");
    }

    public void LogWarning(object? message)
    {
        if (message == null) return;
        PluginLog.Warning($"{message}");
    }
}
