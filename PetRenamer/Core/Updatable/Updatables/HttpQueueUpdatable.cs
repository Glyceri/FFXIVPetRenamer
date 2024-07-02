using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;
using PetRenamer.Core.Networking.NetworkingElements;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Core.Updatable.Updatables;

[Updatable]
internal class HttpQueueUpdatable : Updatable
{
    public override void Update(ref IFramework frameWork, ref IPlayerCharacter player) => HttpRequestQueue.Update(ref frameWork, ref player);
}
