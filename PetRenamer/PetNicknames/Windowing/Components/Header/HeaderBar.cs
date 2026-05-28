using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Windows;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Header;

internal static class HeaderBar
{
    private const float HEADER_BAR_HEIGHT = 35;
    public static float HeaderBarWidth = 0;

    public static void Draw(IPetServices petServices, WindowHandler windowHandler, PetWindow petWindow)
    {
        Vector2 contentSize = ImGui.GetContentRegionAvail();

        contentSize.Y = HEADER_BAR_HEIGHT * WindowHandler.GlobalScale;

        if (!Listbox.Begin($"##headerbar_{WindowHandler.InternalCounter}", contentSize))
        {
            return;
        }

        Vector2 lastPos = ImGui.GetCursorPos();

        ModeToggle.Draw(petWindow);
            
        ImGui.SameLine();

        Vector2 currentCursorPos = ImGui.GetCursorPos();

        string version = petServices.Version;

        ImGui.Text($"v{version}");

        ImGui.SetCursorPos(new Vector2(currentCursorPos.X, lastPos.Y));

        HeaderBarWidth = 0;

        WindowStruct<PetDevWindow>    petDevWindow        = new WindowStruct<PetDevWindow>   (windowHandler, petServices.Configuration, FontAwesomeIcon.Biohazard,  "Pet Dev",                                petServices.Configuration.debugModeActive);
        WindowStruct<KofiWindow>      kofiWindow          = new WindowStruct<KofiWindow>     (windowHandler, petServices.Configuration, FontAwesomeIcon.Coffee,     Translator.GetLine("Kofi.Title"),         petServices.Configuration.showKofiButton && petWindow is not KofiWindow);
        WindowStruct<PetConfigWindow> petConfigWindow     = new WindowStruct<PetConfigWindow>(windowHandler, petServices.Configuration, FontAwesomeIcon.Cogs,       Translator.GetLine("Config.Title"),       petWindow is not PetConfigWindow || petServices.Configuration.quickButtonsToggle);
        WindowStruct<PetListWindow>   petListWindow       = new WindowStruct<PetListWindow>  (windowHandler, petServices.Configuration, FontAwesomeIcon.FileExport, Translator.GetLine("PetList.Sharing"),    petWindow is not PetListWindow && (petServices.Configuration.listButtonLayout == 0 || petServices.Configuration.listButtonLayout == 1));
        WindowStruct<PetListWindow>   actualPetListWindow = new WindowStruct<PetListWindow>  (windowHandler, petServices.Configuration, FontAwesomeIcon.List,       Translator.GetLine("PetList.Title"),      petWindow is not PetListWindow && (petServices.Configuration.listButtonLayout == 0 || petServices.Configuration.listButtonLayout == 2));
        WindowStruct<PetRenameWindow> petRenameWindow     = new WindowStruct<PetRenameWindow>(windowHandler, petServices.Configuration, FontAwesomeIcon.PenSquare,  Translator.GetLine("ContextMenu.Rename"), petWindow is not PetRenameWindow || petServices.Configuration.quickButtonsToggle);

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

readonly ref struct WindowStruct<T> where T : PetWindow
{
    private readonly WindowHandler   WindowHandler;
    private readonly Configuration   Configuration;
    private readonly FontAwesomeIcon Icon;
    private readonly string          Tooltip;
    private readonly bool            Active;

    public WindowStruct(WindowHandler handler, Configuration configuration, FontAwesomeIcon icon, string tooltip, bool active = true)
    {
        Active = active;

        if (Active)
        {
            HeaderBar.HeaderBarWidth += WindowButton.Width;
        }

        WindowHandler = handler;
        Configuration = configuration;
        Icon          = icon;
        Tooltip       = tooltip;
    }

    public void Draw() 
    {
        if (!Active)
        {
            return;
        }

        WindowButton.Draw<T>(in WindowHandler, in Configuration, Icon, Tooltip);

        ImGui.SameLine(0, 0);
    }
}
