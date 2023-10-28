using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System;

namespace PetRenamer.Core.Chat.ChatElements;

[Chat]
internal unsafe class BattleChatElement : ChatElement
{
    internal override void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (!PluginLink.Configuration.useCustomPetNamesInBattleChat) return;
        if (Enum.IsDefined(typeof(XivChatType), type)) return;

        PettableUser user = PluginLink.PettableUserHandler.LastCastedUser()!;
        (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(user, message.ToString(), false);
        StringUtils.instance.ReplaceSeString(ref message, ref validNames);
        return;
    }
}
