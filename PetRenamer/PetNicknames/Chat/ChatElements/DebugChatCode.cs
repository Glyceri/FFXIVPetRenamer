using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using PetRenamer.PetNicknames.Chat.Interfaces;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal class DebugChatCode : IChatElement
{
    readonly Configuration Configuration;

    public DebugChatCode(Configuration configuration)
    {
        Configuration = configuration;
    }

    public void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!Configuration.debugShowChatCode || !Configuration.debugModeActive) return;

        message.Payloads.Insert(0, new TextPayload($"{(int)type}: "));
    }
}
