using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.Core.AutoRegistry.Interfaces;
using PetRenamer.Core.Debug;

namespace PetRenamer.Core.Chat;

internal abstract class ChatElement : IRegistryElement
{
    internal void HandleChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        string originalText = message.ToString();
        string originalSender = sender.ToString();
        if(OnChatMessage(type, senderId, ref sender, ref message, ref isHandled))
            DebugStorage.InsertPetChatMessage(new PetChatMessage(originalText, originalSender, message, sender, type, senderId));
    }
    internal abstract bool OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled);
}
