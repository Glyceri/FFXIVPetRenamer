using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using PetRenamer.Core.Chat.Attributes;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;

namespace PetRenamer.Core.Chat.ChatElements;

[Chat]
internal unsafe class BattleChatElement : RestrictedChatElement
{
    public BattleChatElement() => RegisterChat(2091, 2219, 16427);

    internal override void OnRestrictedChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!PluginLink.Configuration.displayCustomNames) return;
        if (!PluginLink.Configuration.useCustomPetNamesInBattleChat) return;

        PettableUser user = PluginLink.PettableUserHandler.LastCastedUser()!;
        (string, string)[] validNames = PluginLink.PettableUserHandler.GetValidNames(user, message.ToString(), false);
        StringUtils.instance.ReplaceSeString(ref message, ref validNames);
    }
}
