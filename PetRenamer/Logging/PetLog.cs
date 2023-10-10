using PetRenamer.Core.Handlers;
using System;

namespace PetRenamer.Logging;

public static class PetLog
{
    public static void Log(string message) => PluginHandlers.PluginLog.Debug(message);
    public static void LogInfo(string message) => PluginHandlers.PluginLog.Info(message);
    public static void LogWarning(string message) => PluginHandlers.PluginLog.Warning(message);
    public static void LogFatal(string message) => PluginHandlers.PluginLog.Fatal(message);
    public static void LogError(Exception e, string message) => PluginHandlers.PluginLog.Error(e, message);

    public static void Log(object? obj)
    {
        if (obj == null) return;
        PluginHandlers.PluginLog.Debug($"{obj}");
    }

    public static void LogInfo(object? obj)
    {
        if (obj == null) return;
        PluginHandlers.PluginLog.Info($"{obj}");
    }

    public static void LogWarning(object? obj)
    {
        if (obj == null) return;
        PluginHandlers.PluginLog.Warning($"{obj}");
    }

    public static void LogFatal(object? obj)
    {
        if (obj == null) return;
        PluginHandlers.PluginLog.Fatal($"{obj}");
    }
}
