﻿using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Core.Chat.ChatElements;

[Chat]
internal unsafe class BlacklistChatElement : ChatElement
{
    internal override void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        isHandled = PluginLink.ChatHandler.MinusBlacklistCountHandle();
    }
}
