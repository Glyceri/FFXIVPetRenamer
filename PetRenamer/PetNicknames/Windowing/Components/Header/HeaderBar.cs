using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Windows;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Header;

internal static class HeaderBar
{
    const float HEADER_BAR_HEIGHT = 35;
    public static float HeaderBarWidth = 0;

    public static void Draw(in WindowHandler windowHandler, in Configuration configuration, in PetWindow petWindow)
    {
        Vector2 contentSize = ImGui.GetContentRegionAvail();
        contentSize.Y = HEADER_BAR_HEIGHT * ImGuiHelpers.GlobalScale;

        if (Listbox.Begin($"##headerbar_{WindowHandler.InternalCounter}", contentSize))
        {
            Vector2 lastPos = ImGui.GetCursorPos();

            ModeToggle.Draw(petWindow);

            ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPos().X, lastPos.Y));

            HeaderBarWidth = 0;

            WindowStruct<PetDevWindow> petDevWindow = new WindowStruct<PetDevWindow>(in windowHandler, in configuration, FontAwesomeIcon.Biohazard, "Pet Dev", configuration.debugModeActive);
            WindowStruct<KofiWindow> kofiWindow = new WindowStruct<KofiWindow>(in windowHandler, in configuration, FontAwesomeIcon.Coffee, Translator.GetLine("Kofi.Title"), configuration.showKofiButton && petWindow is not KofiWindow);
            WindowStruct<PetConfigWindow> petConfigWindow = new WindowStruct<PetConfigWindow>(in windowHandler, in configuration, FontAwesomeIcon.Cogs, Translator.GetLine("Config.Title"), petWindow is not PetConfigWindow || configuration.quickButtonsToggle);
            WindowStruct<PetListWindow> petListWindow = new WindowStruct<PetListWindow>(in windowHandler, in configuration, FontAwesomeIcon.FileExport, Translator.GetLine("PetList.Sharing"), (petWindow is not PetListWindow) && (configuration.listButtonLayout == 0 || configuration.listButtonLayout == 1));
            WindowStruct<PetListWindow> actualPetListWindow = new WindowStruct<PetListWindow>(in windowHandler, in configuration, FontAwesomeIcon.List, Translator.GetLine("PetList.Title"), (petWindow is not PetListWindow) && (configuration.listButtonLayout == 0 || configuration.listButtonLayout == 2));
            WindowStruct<PetRenameWindow> petRenameWindow = new WindowStruct<PetRenameWindow>(in windowHandler, in configuration, FontAwesomeIcon.PenSquare, Translator.GetLine("ContextMenu.Rename"), petWindow is not PetRenameWindow || configuration.quickButtonsToggle);

            float availableWidth = ImGui.GetContentRegionAvail().X;
            availableWidth -= HeaderBarWidth;

            ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(availableWidth, 0));

            petDevWindow.Draw();
            kofiWindow.Draw();
            petConfigWindow.Draw();
            petListWindow.Draw();
            actualPetListWindow.Draw();
            petRenameWindow.Draw();

            Listbox.End();
        }
    }
}

ref struct WindowStruct<T> where T : PetWindow
{
    readonly WindowHandler WindowHandler;
    readonly Configuration Configuration;
    readonly FontAwesomeIcon Icon;
    readonly string Tooltip;
    readonly bool Active;

    public WindowStruct(in WindowHandler handler, in Configuration configuration, FontAwesomeIcon icon, string tooltip, bool active = true)
    {
        Active = active;

        if (Active) HeaderBar.HeaderBarWidth += WindowButton.Width;

        WindowHandler = handler;
        Configuration = configuration;
        Icon = icon;
        Tooltip = tooltip;
    }

    public void Draw() 
    {
        if (!Active) return;

        WindowButton.Draw<T>(in WindowHandler, in Configuration, Icon, Tooltip);
        ImGui.SameLine(0, 0);
    }
}
