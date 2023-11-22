using PetRenamer.Core.AutoRegistry;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Core.Chat;

internal class ChatHandler : RegistryBase<ChatElement, ChatAttribute>
{
    protected override void OnElementCreation(ChatElement element) =>
        PluginHandlers.ChatGui.ChatMessage += element.OnChatMessage;
    
    protected override void OnElementDestroyed(ChatElement element) =>
        PluginHandlers.ChatGui.ChatMessage -= element.OnChatMessage;

    public int BlacklistCount { get; private set; } = 0;
    internal void AddBlacklistedChats(int amount) => BlacklistCount += amount;
    internal void RemoveBlacklistedChats(int amount) => BlacklistCount -= amount;
    internal bool MinusBlacklistCountHandle()
    {
        bool outcome = BlacklistCount-- > 0;
        if (BlacklistCount < 0) BlacklistCount = 0;
        return outcome;
    }
}
