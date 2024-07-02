using Dalamud.Plugin.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PetLogWrapper : IPetLog
{
    IPluginLog pluginLog;
    public PetLogWrapper(IPluginLog pluginLog) => this.pluginLog = pluginLog;

    public void Log(object? message)
    {
        if (message == null) return;
        pluginLog.Debug($"{message}");
    }

    public void LogError(Exception e, object? message)
    {
        if (message == null) return;
        pluginLog.Error($"{message}");
    }

    public void LogFatal(object? message)
    {
        if (message == null) return;
        pluginLog.Fatal($"{message}");
    }

    public void LogInfo(object? message)
    {
        if (message == null) return;
        pluginLog.Info($"{message}");
    }

    public void LogWarning(object? message)
    {
        if (message == null) return;
        pluginLog.Warning($"{message}");
    }
}
