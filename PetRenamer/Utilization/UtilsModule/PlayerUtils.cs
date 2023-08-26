using Dalamud.Game.ClientState.Objects.SubKinds;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class PlayerUtils : UtilsRegistryType, ISingletonBase<PlayerUtils>
{
    public static PlayerUtils instance { get; set; } = null!;

    public bool PlayerDataAvailable() => PluginHandlers.ClientState.LocalPlayer != null;
    public PlayerCharacter? PlayerCharacter => PluginHandlers.ClientState.LocalPlayer;
}