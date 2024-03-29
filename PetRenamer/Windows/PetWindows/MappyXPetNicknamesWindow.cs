﻿using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;
using System.Numerics;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
internal class MappyXPetNicknamesWindow : PluginXPetwindow
{
    public MappyXPetNicknamesWindow() : base("Mappy X Pet Nicknames", "Mappy", "Mappy User", @"https://raw.githubusercontent.com/goatcorp/PluginDistD17/main/stable/Mappy/images/Icon.png", "Enable Mappy", "Clicking this button will disable the Pet Module in Mappy and allow Pet Nicknames to handle Pets in Mappy!", OnButtonClick) { }

    public void TryOpen()
    {
        if (PluginLink.Configuration.readMappyIntegration) return;
        PluginLink.Configuration.readMappyIntegration = true;
        PluginLink.Configuration.Save();
        IsOpen = true;
    }

    static void OnButtonClick()
    {
        PluginLink.Configuration.enableMappyIntegration = true;
        PluginLink.Configuration.Save();
        PluginLink.WindowHandler.GetWindow<MappyXPetNicknamesWindow>().IsOpen = false;
    }

    public override void OnXDraw()
    {
        OverrideLabel("Mappy is now integrated into Pet Nicknames!", new Vector2(ContentAvailableX, BarSize));
        OverrideLabel("By clicking Enable Mappy you will automatically disable", new Vector2(ContentAvailableX, BarSize)); 
        OverrideLabel("Mappy handling Pets, and Allow Pet Nicknames", new Vector2(ContentAvailableX, BarSize));
        OverrideLabel("to display Custom Nicknames. Press the [X] button if you", new Vector2(ContentAvailableX, BarSize));
        OverrideLabel("don't want any of that.", new Vector2(ContentAvailableX, BarSize));
    }
}
