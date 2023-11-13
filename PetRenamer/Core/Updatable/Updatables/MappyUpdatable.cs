using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Ipc.MappyIPC;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class MappyUpdatable : Updatable
{
    public override unsafe void Update(ref IFramework frameWork, ref PlayerCharacter player) => IPCMappy.Update(ref frameWork, ref player);
}