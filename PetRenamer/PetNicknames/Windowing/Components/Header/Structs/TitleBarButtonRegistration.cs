using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Base;
using System;

namespace PetRenamer.PetNicknames.Windowing.Components.Header.Structs;

// CreateTitleBarButton<PetDevWindow>   (windowHandler, petServices, "PetDev.Title",       FontAwesomeIcon.Biohazard,  petServices.Configuration.debugModeActive);

internal abstract class TitleBarButtonRegistration
{
    public readonly string                     TitleKey;
    public readonly FontAwesomeIcon            Icon;
    public readonly Func<Configuration, bool>? ButtonValidator;
    
    public TitleBarButtonRegistration(string titleKey, FontAwesomeIcon icon, Func<Configuration, bool>? buttonValidator = null)
    {
        TitleKey        = titleKey;
        Icon            = icon;
        ButtonValidator = buttonValidator;
    }
    
    public abstract void HandleClick(WindowHandler windowHandler, Configuration configuration, ImGuiMouseButton mouseButton);
    public abstract bool IsOpen(WindowHandler windowHandler);
}

internal class TitleBarButtonRegistration<T> : TitleBarButtonRegistration
    where T : PetWindow
{
    public TitleBarButtonRegistration(string titleKey, FontAwesomeIcon icon, Func<Configuration, bool>? buttonValidator = null) 
        : base(titleKey, icon, buttonValidator)
        { }

    public override void HandleClick(WindowHandler windowHandler, Configuration configuration, ImGuiMouseButton mouseButton)
    {
        if (mouseButton == ImGuiMouseButton.Left)
        {
            if (configuration.quickButtonsToggle)
            {
                windowHandler.GetWindow<T>()?.Toggle();
            }
            else
            {
                windowHandler.GetWindow<T>()?.Open();
            }
        }
                
        if (mouseButton == ImGuiMouseButton.Right)
        {
            windowHandler.GetWindow<T>()?.Close();
        }
                
        if (mouseButton == ImGuiMouseButton.Middle)
        {
            windowHandler.GetWindow<T>()?.Toggle();
        }
    }

    public override bool IsOpen(WindowHandler windowHandler) 
        => windowHandler.GetWindow<T>()?.IsOpen ?? false;
}