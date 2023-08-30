using System.Numerics;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Translations;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[ConfigPetWindow]
[PersistentPetWindow]
public class ConfigWindow : PetWindow
{
    public ConfigWindow() : base(
        Translate.GetValue("Global_Minionname_Settings"),
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse)
    {
        Size = new Vector2(232, 225);
        SizeCondition = ImGuiCond.Always;
    }

    public override void OnDraw()
    {
        if (Checkbox("Display_Custom_Names", ref PluginLink.Configuration.displayCustomNames) || 
            Checkbox("Use_Custom_Theme", ref PluginLink.Configuration.useCustomTheme) || 
            Checkbox("Allow_Tooltips", ref PluginLink.Configuration.allowTooltips) || 
            Checkbox("Use_Custom_Names_For_Emotes", ref PluginLink.Configuration.replaceEmotes) || 
            Checkbox("Allow_Context_Menus", ref PluginLink.Configuration.useContextMenus))
            PluginLink.Configuration.Save();


        if (Button("Clear_All_Nicknames")) 
            PluginLink.WindowHandler.AddTemporaryWindow<ConfirmPopup>(
                "Clear_Nicknames_Confirm",
                (outcome) => { if ((bool)outcome) { PluginLink.Configuration.ClearAllNicknames(); } }
                , this);

        if (Button("Credits"))
            PluginLink.WindowHandler.OpenWindow<CreditsWindow>();
    }
}
