using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Text.Json.Serialization;

namespace PetRenamer;

[Serializable]
public class Configuration : IPluginConfiguration
{
    [JsonIgnore]
    IDalamudPluginInterface? PetNicknamesPlugin;
    [JsonIgnore]
    public const int currentSaveFileVersion = 9;
    public int Version { get; set; } = currentSaveFileVersion;

    // ------------------------ Unrelated Settings -----------------------
    public bool limitLocalSearch = false;
    // ------------------------- Global Settings -------------------------
    public bool displayCustomNames = true;
    public bool downloadProfilePictures = false;
    public bool displayImages = true;
    public bool automaticallySwitchPetmode = true;
    public bool disablePVPChatMessage = false;
    // ----------------------- Battle Pet Settings -----------------------
    public bool allowCastBarPet = true;
    public bool useCustomFlyoutPet = true;
    public bool useCustomPetNamesInBattleChat = true;
    public bool useCustomPetNamesInInfoChat = true;
    public bool useContextMenuOnBattlePets = true;
    public bool allowTooltipsBattlePets = true;
    public bool replaceEmotesBattlePets = true;
    // ------------------------- Minion Settings -------------------------
    public bool useContextMenuOnMinions = true;
    public bool allowTooltipsOnMinions = true;
    public bool replaceEmotesOnMinions = true;
    public bool showNamesInMinionBook = true;
    // ---------------------- Sharing Mode Settings ----------------------
    public bool alwaysOpenAdvancedMode = false;
    // --------------------------- UI SETTINGS ---------------------------
    public bool anonymousMode = false;
    public bool spaceOutSettings = false;
    public bool startSettingsOpen = false;
    public bool quickButtonsToggle = false;
    public bool newUseCustomTheme = false;
    public bool showKofiButton = true;
    public string activeElement = "Event";
    public bool hideHelpButton = false;
    public bool hidePetListButton = false;
    // -------------------------- DEBUG SETTINGS -------------------------
    public bool debugMode = false;
    public bool autoOpenDebug = true;
    public bool showChatID = false;

    public void Initialise(ref IDalamudPluginInterface PetNicknamesPlugin)
    {
        this.PetNicknamesPlugin = PetNicknamesPlugin;
        CurrentInitialise();
    }

    void CurrentInitialise()
    {
        
    }

    public void Save() 
    {
        if (currentSaveFileVersion < Version) return;
        PetNicknamesPlugin?.SavePluginConfig(this); 
    }
}
