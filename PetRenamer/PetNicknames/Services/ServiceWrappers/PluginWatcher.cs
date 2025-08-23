using Dalamud.Plugin;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PluginWatcher : IPluginWatcher
{
    private readonly DalamudServices        DalamudServices;

    private readonly List<string>           LoadedPlugins       = new List<string>();
    private readonly List<Action<string[]>> RegisteredCallbacks = new List<Action<string[]>>();

    public PluginWatcher(DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;

        FloodList();

        DalamudServices.DalamudPlugin.ActivePluginsChanged += OnPluginsChanged;
    }

    public void RegisterListener(Action<string[]> callback)
    {
        _ = RegisteredCallbacks.Remove(callback);

        RegisteredCallbacks.Add(callback);
    }

    public void DeregisterListener(Action<string[]> callback)
    {
        _ = RegisteredCallbacks.Remove(callback);
    }

    private void OnPluginsChanged(IActivePluginsChangedEventArgs args)
    {
        foreach (string internalName in args.AffectedInternalNames)
        {
            if (args.Kind == PluginListInvalidationKind.Unloaded)
            {
                OnPluginUnloaded(internalName);
            }
            else
            {
                OnPluginLoaded(internalName);
            }
        }
    }

    private void FloodList()
    {
        foreach (IExposedPlugin plugin in DalamudServices.DalamudPlugin.InstalledPlugins)
        {
            if (!plugin.IsLoaded)
            {
                continue;
            }

            OnPluginLoaded(plugin.InternalName);
        }
    }

    private void OnPluginLoaded(string internalName)
    {
        bool pluginWasInList = LoadedPlugins.Remove(internalName);

        LoadedPlugins.Add(internalName);

        if (pluginWasInList)
        {
            return;
        }

        NotifyChange();
    }

    private void OnPluginUnloaded(string internalName)
    {
        bool pluginWasInList = LoadedPlugins.Remove(internalName);

        if (!pluginWasInList)
        {
            return;
        }

        NotifyChange();
    }

    private void NotifyChange()
    {
        string[] loadedPlugins = LoadedPlugins.ToArray();

        foreach (Action<string[]> callback in RegisteredCallbacks)
        {
            callback?.Invoke(loadedPlugins);
        }
    }

    public void Dispose()
    {
        DalamudServices.DalamudPlugin.ActivePluginsChanged -= OnPluginsChanged;
    }
}
