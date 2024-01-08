using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using System.Linq;

namespace PetRenamer.Core.Chat.ChatElements;

[Chat]
internal unsafe class InfoChatMessage : ChatElement
{
    // 2106 Give carbuncle the order:
    // 2105 Carbuncle withdraws

    readonly int[] allowedChatMessages = new int[] { 2105, 2106 };

    internal override void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (!PluginLink.Configuration.useCustomPetNamesInInfoChat) return;

        int typeAsInt = (int)type;
        if (!allowedChatMessages.Contains(typeAsInt)) return;

        PettableUser user = PluginLink.PettableUserHandler.LocalUser()!;
        (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(user, message.ToString(), false);
        StringUtils.instance.ReplaceSeString(ref message, ref validNames);
        return;
    }
}
