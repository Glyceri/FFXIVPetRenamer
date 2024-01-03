using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Environment.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Singleton;
using PetRenamer.Logging;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Core.Environment.EnvironmentElements;

[Environment]
internal class PluginChangedElement : EnvironmentElement, ISingletonBase<PluginChangedElement>
{
    public static PluginChangedElement instance { get; set; } = null!;

    internal override void OnInitialize() 
    { 
        PetLog.Log("INITIALIZED!");
        PluginHandlers.PluginInterface.ActivePluginsChanged += OnChanged; 
    }
    internal override void OnDispose() => PluginHandlers.PluginInterface.ActivePluginsChanged -= OnChanged;

    void OnChanged(PluginListInvalidationKind kind, bool affectedThisPlugin)
    {
        PetLog.Log("CHANGED!!!");
    }

    double timer = 1;
    const double checkupTimer = 10;

    InstalledPluginState[] lastArray = new InstalledPluginState[0];

    /*
    void Update(IFramework framework)
    {
        
        timer += framework.UpdateDelta.TotalSeconds;
        if (timer >= checkupTimer)
        {
            timer -= checkupTimer;
            LoopThroughPlugins();
        }
    }
    */
    void LoopThroughPlugins() 
    {
        InstalledPluginState[] plugins = PluginHandlers.PluginInterface.InstalledPlugins.ToArray();

        lastArray = plugins.ToArray();
    }

    public void OnPluginsChanged()
    {
        PetLog.Log("CHANGE!");
    }
}
