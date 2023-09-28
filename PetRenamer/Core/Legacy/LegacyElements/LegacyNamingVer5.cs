#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using Dalamud.Game.ClientState.Objects.SubKinds;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Legacy.Attributes;

namespace PetRenamer.Core.Legacy.LegacyElements;

[Legacy(new int[1] { 5 })]
internal class LegacyNamingVer5 : LegacyElement
{
    internal override void OnPlayerAvailable(int detectedVersion, ref PlayerCharacter player)
    {
        if (detectedVersion != 5) return;

        PluginLink.Configuration.allowCastBarPet = PluginLink.Configuration.allowCastBar;
        PluginLink.Configuration.useCustomFlyoutPet = PluginLink.Configuration.useCustomFlyoutInChat;
        PluginLink.Configuration.useCustomPetNamesInBattleChat = PluginLink.Configuration.useCustomNamesInChat;
        PluginLink.Configuration.useContextMenuOnBattlePets = PluginLink.Configuration.useContextMenus;
        PluginLink.Configuration.allowTooltipsBattlePets = PluginLink.Configuration.allowTooltips;
        PluginLink.Configuration.replaceEmotesBattlePets = PluginLink.Configuration.replaceEmotes;

        PluginLink.Configuration.useContextMenuOnMinions = PluginLink.Configuration.useContextMenus;
        PluginLink.Configuration.allowTooltipsOnMinions = PluginLink.Configuration.allowTooltips;
        PluginLink.Configuration.replaceEmotesOnMinions = PluginLink.Configuration.replaceEmotes;

        PluginLink.Configuration.Version = 6;
        PluginLink.Configuration.Save();
    }
}
#pragma warning restore CS0618 // Type or member is obsolete