using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Components.Header.Structs;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Windowing.Windows;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Header;

internal static class HeaderBar
{
    private const float HEADER_BAR_HEIGHT = 35;
    
    private static readonly List<TitleBarButton>             _titleBarButtons = [];
    private static readonly List<TitleBarButtonRegistration> Registrations    =
    [
        new TitleBarButtonRegistration<PetDevWindow>    ("PetDev.Title",        FontAwesomeIcon.Biohazard,  configuration => configuration.debugModeActive),
        new TitleBarButtonRegistration<KofiWindow>      ("Kofi.Title",          FontAwesomeIcon.Coffee,     configuration => configuration.showKofiButton),
        new TitleBarButtonRegistration<PetConfigWindow> ("Config.Title",        FontAwesomeIcon.Cog),
        new TitleBarButtonRegistration<PetListWindow>   ("PetList.Sharing",     FontAwesomeIcon.FileExport, configuration => configuration.listButtonLayout is 0 or 1),
        new TitleBarButtonRegistration<PetListWindow>   ("PetList.Title",       FontAwesomeIcon.List,       configuration => configuration.listButtonLayout is 0 or 2),
        new TitleBarButtonRegistration<PetRenameWindow> ("ContextMenu.Rename",  FontAwesomeIcon.PenSquare)
    ];
    
    private static int priority;
    
    public static void Draw(IPetWindow window, IPetServices petServices, WindowHandler windowHandler)
    {
        if (!window.HasModeToggle && !window.ShowQuickButtons)
        {
            return;
        }
        
        Vector2 contentSize = ImGui.GetContentRegionAvail();

        contentSize.Y = HEADER_BAR_HEIGHT * WindowHandler.GlobalScale;

        if (!Listbox.Begin($"##headerbar_{WindowHandler.InternalCounter}", contentSize))
        {
            return;
        }
        
        ModeToggle.Draw(window, petServices);
        
        if (!petServices.Configuration.useNewBarStyle && window.ShowQuickButtons)
        {
            ImGui.SameLine(0, 0);
            
            int[] indexes   = GetValidIndexes(petServices);
            
            float newY      = ImGui.GetContentRegionAvail().Y * 0.25f;
            
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - newY);
            
            float newX      = ImGui.GetContentRegionAvail().X - (indexes.Length * ImGui.GetContentRegionAvail().Y);
            
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + newX);

            int arraySize   = indexes.Length;
            
            for (int i = 0; i < arraySize; i++)
            {
                int index = indexes[i];
                
                DrawButton(index, petServices, windowHandler);
                
                if (i == arraySize - 1)
                {
                    continue;
                }
                
                ImGui.SameLine(0, 0);
                
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() - newY);
            }
        }
        
        Listbox.End();
    }
    
    private static void DrawButton(int index, IPetServices petServices, WindowHandler windowHandler)
    {
        TitleBarButtonRegistration registration = Registrations[index];
        
        bool isActive = registration.IsOpen(windowHandler);
        
        ImGui.BeginDisabled(isActive && !petServices.Configuration.quickButtonsToggle);

        float size = ImGui.GetContentRegionAvail().Y;

        ImGui.PushFont(UiBuilder.IconFont);
        
        TextAligner.Align(TextAlignment.Centre);
        
        bool shouldDoWindow = ImGui.Button($"{registration.Icon.ToIconString()}##quickButton_{WindowHandler.InternalCounter}", new Vector2(size, size));
        
        TextAligner.PopAlignment();
        
        ImGui.PopFont();

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(Translator.GetLine(registration.TitleKey));
        }

        ImGui.EndDisabled();

        if (!shouldDoWindow)
        {
            return;
        }
        
        registration.HandleClick(windowHandler, petServices.Configuration, ImGuiMouseButton.Left);
    }
    
    private static int[] GetValidIndexes(IPetServices petServices)
    {
        List<int> validIndexes = [];
        int       index        = -1;
        
        foreach (TitleBarButtonRegistration registration in Registrations)
        {
            index++;
            
            if (!(registration.ButtonValidator?.Invoke(petServices.Configuration) ?? true))
            {
                continue;
            } 
            
            validIndexes.Add(index);
        }
        
        return [.. validIndexes];
    }
    
    public static List<TitleBarButton> HandleHeaderButtons(WindowHandler windowHandler, IPetServices petServices)
    {
        priority = 0;
        
        _titleBarButtons.Clear();
        
        foreach (int index in GetValidIndexes(petServices))
        {
            CreateTitleBarButton(Registrations[index], windowHandler, petServices);
        }
            
        return _titleBarButtons;
    }
    
    private static void CreateTitleBarButton(TitleBarButtonRegistration titleButton, WindowHandler windowHandler, IPetServices petServices)
    {
        if (!(titleButton.ButtonValidator?.Invoke(petServices.Configuration) ?? true))
        {
            return;
        } 
        
        _titleBarButtons.Add(new TitleBarButton
        {
            Icon        = titleButton.Icon,
            IconOffset  = new(0, 1),
            ShowTooltip = () =>
            {
                using ImRaii.TooltipDisposable tooltip = ImRaii.Tooltip();
                
                ImGui.Text(Translator.GetLine(titleButton.TitleKey));
            },
            Click = button =>
            {
                titleButton.HandleClick(windowHandler, petServices.Configuration, button);
            },
            Priority = priority++,
        });
    }
}