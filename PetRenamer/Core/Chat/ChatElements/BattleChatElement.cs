using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System;

namespace PetRenamer.Core.Chat.ChatElements;

// TODO: FIX
[Chat]
internal unsafe class BattleChatElement : ChatElement
{
    internal override bool OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PluginLink.Configuration.displayCustomNames) return false;
        if (!PluginLink.Configuration.useCustomPetNamesInBattleChat) return false;
        if (Enum.IsDefined(typeof(XivChatType), type)) return false;

        PettableUser user = PluginLink.PettableUserHandler.LastCastedUser()!;
        (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(user, message.ToString(), false);
        StringUtils.instance.ReplaceSeString(ref message, ref validNames);
        return true;
    }
}
