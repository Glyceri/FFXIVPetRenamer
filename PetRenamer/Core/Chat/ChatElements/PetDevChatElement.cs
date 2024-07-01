using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Core.Chat.ChatElements;

[Chat]
internal class PetDevChatElement : ChatElement
{
    internal override void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PluginLink.Configuration.debugMode || !PluginLink.Configuration.showChatID) return;

        message.Payloads.Insert(0, new TextPayload(((int)type).ToString() + ": "));
    }
}
