#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer5 : ILegacyStepperElement
{
    public int OldVersion { get; } = 5;

    public void Upgrade(Configuration configuration)
    {
        configuration.allowCastBarPet = configuration.allowCastBar;
        configuration.useCustomFlyoutPet = configuration.useCustomFlyoutInChat;
        configuration.useCustomPetNamesInBattleChat = configuration.useCustomNamesInChat;
        configuration.useContextMenuOnBattlePets = configuration.useContextMenus;
        configuration.allowTooltipsBattlePets = configuration.allowTooltips;
        configuration.replaceEmotesBattlePets = configuration.replaceEmotes;

        configuration.useContextMenuOnMinions = configuration.useContextMenus;
        configuration.allowTooltipsOnMinions = configuration.allowTooltips;
        configuration.replaceEmotesOnMinions = configuration.replaceEmotes;

        configuration.Version = 6;
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
