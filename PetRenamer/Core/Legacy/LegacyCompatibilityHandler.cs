using Dalamud.Game;
using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Legacy.Attributes;
using PetRenamer.Utilization.UtilsModule;
using System.Collections.Generic;
using System.Linq;

namespace PetRenamer.Core.Legacy;

internal class LegacyCompatibilityHandler : RegistryBase<LegacyElement, LegacyAttribute>
{ 
    int lastInternalVersion = -1;
    int currentInternalVersion = 0;

    List<LegacyElement> correctElements = new List<LegacyElement>();

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

    internal void OnUpdate(Framework frameWork)
    {
        if(lastInternalVersion != currentInternalVersion)
        {
            Reset();
            lastInternalVersion = currentInternalVersion;
        }

        foreach (LegacyElement legacyElement in correctElements)
            legacyElement.OnUpdate(frameWork, currentInternalVersion);

        if (hasFoundPlayer) return;

        hasFoundPlayer = PlayerUtils.instance.PlayerDataAvailable();
        if (hasFoundPlayer)
            foreach (LegacyElement legacyElement in correctElements)
                legacyElement.OnPlayerAvailable(currentInternalVersion);
        
    }
}