using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Core.Chat;

internal class ChatHandler : RegistryBase<ChatElement, ChatAttribute>
{

    protected override void OnElementCreation(ChatElement element)
    {
        PluginHandlers.ChatGui.ChatMessage += element.OnChatMessage;
    }


    protected override void OnElementDestroyed(ChatElement element)
    {
        PluginHandlers.ChatGui.ChatMessage -= element.OnChatMessage;
    }
}
