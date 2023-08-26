using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.SubKinds;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Core.Legacy.Attributes;

internal class LegacyElement : IRegistryElement 
{
    internal virtual void OnStartup(int detectedVersion) { }
    internal virtual void OnPlayerAvailable(PlayerCharacter playerData, int detectedVersion) { }
    internal virtual void OnUpdate(Framework frameWork, int detectedVersion) { }
}