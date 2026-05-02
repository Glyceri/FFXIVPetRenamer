using Dalamud.Game.Chat;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using PetRenamer.PetNicknames.Chat.Interfaces;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal class DebugChatCode : IChatElement
{
    private readonly Configuration Configuration;

    public DebugChatCode(Configuration configuration)
    {
        Configuration = configuration;
    }

    public void OnChatMessage(IHandleableChatMessage chatMessage)
    {
        if (!Configuration.debugShowChatCode || !Configuration.debugModeActive)
        {
            return;
        }

        chatMessage.Message.Payloads.Insert(0, new TextPayload($"{(int)chatMessage.LogKind}: "));
    }
}
