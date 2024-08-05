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
            ModeToggle.Draw(petWindow);

            ImGui.SameLine(0, 0);

            HeaderBarWidth = 0;

            WindowStruct<PetRenameWindow> petRenameWindow = new WindowStruct<PetRenameWindow>(in windowHandler, in configuration, FontAwesomeIcon.PenSquare, Translator.GetLine("ContextMenu.Rename"));
            WindowStruct<PetListWindow> petListWindow = new WindowStruct<PetListWindow>(in windowHandler, in configuration, FontAwesomeIcon.FileExport, Translator.GetLine("PetList.Sharing"));
            WindowStruct<PetConfigWindow> petConfigWindow = new WindowStruct<PetConfigWindow>(in windowHandler, in configuration, FontAwesomeIcon.Cogs, Translator.GetLine("Config.Title"));

            float availableWidth = ImGui.GetContentRegionAvail().X;
            availableWidth -= HeaderBarWidth;
            
            ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(availableWidth, 0));

            petRenameWindow.Draw();
            petListWindow.Draw();
            petConfigWindow.Draw();

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

    public WindowStruct(in WindowHandler handler, in Configuration configuration, FontAwesomeIcon icon, string tooltip)
    {
        HeaderBar.HeaderBarWidth += WindowButton.Width;

        WindowHandler = handler;
        Configuration = configuration;
        Icon = icon;
        Tooltip = tooltip;
    }

    public void Draw() 
    {
        WindowButton.Draw<T>(in WindowHandler, in Configuration, Icon, Tooltip);
        ImGui.SameLine(0, 0);
    }
}
