using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.Chat.ChatElements;

[Chat]
internal unsafe class InfoChatMessage : RestrictedChatElement
{
    // 2106 Give carbuncle the order:
    // 2105 Carbuncle withdraws

    public InfoChatMessage() => RegisterChat(2105, 2106);

    internal override void OnRestrictedChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    { 
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (!PluginLink.Configuration.useCustomPetNamesInInfoChat) return;

        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(user, message.ToString(), false);
        StringUtils.instance.ReplaceSeString(ref message, ref validNames);
    }
}
