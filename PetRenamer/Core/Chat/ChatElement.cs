using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Core.Chat;

internal abstract class ChatElement : IRegistryElement
{
    internal abstract void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled);
    internal virtual void OnChatMessageHandled(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled) { }
}
