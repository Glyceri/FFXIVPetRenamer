using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Core.Legacy.Attributes;

internal class LegacyElement : IRegistryElement 
{
    internal virtual void OnStartup(int detectedVersion) { }
    internal virtual void OnPlayerAvailable(int detectedVersion, ref PlayerCharacter player) { }
    internal virtual void OnUpdate(IFramework frameWork, int detectedVersion) { }
}