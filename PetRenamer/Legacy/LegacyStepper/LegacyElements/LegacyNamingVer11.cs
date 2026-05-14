using PetRenamer.Legacy.LegacyStepper.LegacyElements.Interfaces;

#pragma warning disable CS0618 // Type or member is obsolete. By nature of Legacy Support they are always obsolete.
#pragma warning disable CS0612 // Type or member is obsolete

namespace PetRenamer.Legacy.LegacyStepper.LegacyElements;

internal class LegacyNamingVer11 : ILegacyStepperElement
{
    public int OldVersion 
        => 11;
    
    public void Upgrade(Configuration configuration)
    {
        configuration.Version                               = 12;

        configuration.SelectedColourMode                    = (Configuration.ColourMode)configuration.showColours;
    
        configuration.ShowOnNameplatesColour.Enabled        = configuration.showOnNameplates;
        configuration.ShowOnCastbarsColour.Enabled          = configuration.showOnCastbars;
        configuration.ShowInBattleChatColour.Enabled        = configuration.showInBattleChat;
        configuration.ShowOnFlyoutColour.Enabled            = configuration.showOnFlyout;
        configuration.ShowOnEmotesColour.Enabled            = configuration.showOnEmotes;
        configuration.ShowOnTooltipColour.Enabled           = configuration.showOnTooltip;
        configuration.ShowNamesInMinionBookColour.Enabled   = configuration.showNamesInMinionBook;
        configuration.ShowNamesInActionLogColour.Enabled    = configuration.showNamesInActionLog;
        configuration.ShowOnTargetBarsColour.Enabled        = configuration.showOnTargetBars;
        configuration.ShowOnPartyListColour.Enabled         = configuration.showOnPartyList;
    }
}

#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0618 // Type or member is obsolete
