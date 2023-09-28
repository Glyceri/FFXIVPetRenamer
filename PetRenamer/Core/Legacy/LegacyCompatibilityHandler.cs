using Dalamud.Game;
using Dalamud.Plugin.Services;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Legacy.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Core.Legacy;

internal class LegacyCompatibilityHandler : RegistryBase<LegacyElement, LegacyAttribute>
{ 
    int lastInternalVersion = -1;
    readonly int currentInternalVersion = 0;

    readonly List<LegacyElement> correctElements = new List<LegacyElement>();

    public LegacyCompatibilityHandler() : base()
    {
        currentInternalVersion = PluginLink.Configuration.Version;
    }

    bool hasFoundPlayer = false;

    void Reset()
    {
        hasFoundPlayer = false;
        correctElements.Clear();
        for(int i = 0; i < elements.Count; i++)
        {
            LegacyElement element = elements[i];
            LegacyAttribute attribute = attributes[i];
            if(attribute.forVersions.Contains(currentInternalVersion))
                correctElements.Add(element);
        }

        foreach(LegacyElement legacyElement in correctElements)
            legacyElement.OnStartup(currentInternalVersion);
    }

    internal void OnUpdate(IFramework frameWork)
    {
        if(lastInternalVersion != currentInternalVersion)
        {
            Reset();
            lastInternalVersion = currentInternalVersion;
        }

        foreach (LegacyElement legacyElement in correctElements)
            legacyElement.OnUpdate(frameWork, currentInternalVersion);

        if (hasFoundPlayer) return;

        hasFoundPlayer = PluginHandlers.ClientState.LocalPlayer != null;
        if (hasFoundPlayer)
            foreach (LegacyElement legacyElement in correctElements)
                legacyElement.OnPlayerAvailable(currentInternalVersion);
        
    }
}
