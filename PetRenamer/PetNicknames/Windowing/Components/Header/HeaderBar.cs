using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Windows;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Header;

internal static class HeaderBar
{
    private static int  priority;
    
    private static readonly List<TitleBarButton> titleBarButtons = [];
    
    public static List<TitleBarButton> HandleHeaderButtons(WindowHandler windowHandler, IPetServices petServices)
    {
        priority = 0;
        titleBarButtons.Clear();
        
        CreateTitleBarButton<PetDevWindow>   (windowHandler, petServices, "PetDev.Title",       FontAwesomeIcon.Biohazard,  petServices.Configuration.debugModeActive);
        CreateTitleBarButton<KofiWindow>     (windowHandler, petServices, "Kofi.Title",         FontAwesomeIcon.Coffee,     petServices.Configuration.showKofiButton);
        CreateTitleBarButton<PetConfigWindow>(windowHandler, petServices, "Config.Title",       FontAwesomeIcon.Cog);
        CreateTitleBarButton<PetListWindow>  (windowHandler, petServices, "PetList.Sharing",    FontAwesomeIcon.FileExport, petServices.Configuration.listButtonLayout is 0 or 1);
        CreateTitleBarButton<PetListWindow>  (windowHandler, petServices, "PetList.Title",      FontAwesomeIcon.List,       petServices.Configuration.listButtonLayout is 0 or 2);
        CreateTitleBarButton<PetRenameWindow>(windowHandler, petServices, "ContextMenu.Rename", FontAwesomeIcon.PenSquare);
        
        return titleBarButtons;
    }
    
    private static void CreateTitleBarButton<T>(WindowHandler windowHandler, IPetServices petServices, string tooltipUntranslated, FontAwesomeIcon icon, bool isActive = true)
        where T : PetWindow 
    {
        if (!isActive)
        {
            return;
        } 
        
        titleBarButtons.Add(new TitleBarButton
        {
            Icon        = icon,
            IconOffset  = new(0, 1),
            ShowTooltip = () =>
            {
                using ImRaii.TooltipDisposable tooltip = ImRaii.Tooltip();
                
                ImGui.Text(Translator.GetLine(tooltipUntranslated));
            },
            Click = button =>
            {
                if (button == ImGuiMouseButton.Left)
                {
                    if (petServices.Configuration.quickButtonsToggle)
                    {
                        windowHandler.GetWindow<T>()?.Toggle();
                    }
                    else
                    {
                        windowHandler.GetWindow<T>()?.Open();
                    }
                }
                
                if (button == ImGuiMouseButton.Right)
                {
                    windowHandler.GetWindow<T>()?.Close();
                }
                
                if (button == ImGuiMouseButton.Middle)
                {
                    windowHandler.GetWindow<T>()?.Toggle();
                }
            },
            Priority = priority++,
        });
    }
}