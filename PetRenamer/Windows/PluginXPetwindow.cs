using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Networking.NetworkingElements;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Windows.PetWindows;
using System;
using System.Numerics;

namespace PetRenamer.Windows;

internal class PluginXPetwindow : PetWindow
{
    static PettableUser petRenamerUser = null!;
    PettableUser pluginUser = null!;

    string pluginU;
    string pluginIconURL;
    bool handledAlready = false;
    string pluginName;
    string bottomButtonText;
    string bottomButtonTooltip;
    Action callback;

    public PluginXPetwindow(string name, string pluginName, string pluginU, string pluginIconURL, string bottomButtonText, string bottomButtonTooltip, Action callback) : base(name, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoNavInputs, false) 
    {
        this.pluginName = pluginName;   
        this.pluginU = pluginU;
        this.pluginIconURL = pluginIconURL;
        this.bottomButtonText = bottomButtonText;
        this.bottomButtonTooltip = bottomButtonTooltip;
        this.callback = callback;
        Size = new Vector2(400, 400);
    }

    public override void OnWindowOpen()
    {
        if (handledAlready) return;
        handledAlready = true;
        if (petRenamerUser == null)
        {
            petRenamerUser = new PettableUser("Pet Nicknames", ushort.MinValue);
            NetworkedImageDownloader.instance.AsyncDownload(@"https://raw.githubusercontent.com/goatcorp/PluginDistD17/main/stable/PetRenamer/images/icon.png", (petRenamerUser.UserName, petRenamerUser.Homeworld));
        }
        pluginUser = new PettableUser(pluginU, ushort.MinValue);
        NetworkedImageDownloader.instance.AsyncDownload(pluginIconURL, (pluginUser.UserName, pluginUser.Homeworld));
    }

    public sealed override void OnDraw()
    {
        if (pluginUser == null || petRenamerUser == null) return;
        if (!BeginListBox($"##<MappyBox>{internalCounter++}", new Vector2(ContentAvailableX, 126))) return;
        OverrideLabel($"{pluginName} X Pet Nicknames", new Vector2(ContentAvailableX, BarSize));
        ImGui.NewLine();
        ImGui.SameLine(0, 60);
        DrawUserTextureEncased(pluginUser, false);
        ImGui.SameLine(0, 195 - 120);
        DrawUserTextureEncased(petRenamerUser, false);
        SameLineNoMargin();
        ImGui.SetCursorPos(ImGui.GetCursorPos() - new Vector2(140, -35));
        Label("X");
        ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(140, -35));

        ImGui.EndListBox();
        if (!BeginListBoxSub($"##<MappyBox2>{internalCounter++}", new Vector2(ContentAvailableX, 200))) return;
        if (PluginLink.Configuration.understoodWarningThirdPartySettings) OnXDraw();
        else DrawThirdPartyWarning();
        ImGui.EndListBox();
        if (PluginLink.Configuration.understoodWarningThirdPartySettings)
            Button(bottomButtonText, new Vector2(ContentAvailableX, BarSize), bottomButtonTooltip, callback);
    }

    void DrawThirdPartyWarning() => PluginLink.WindowHandler.GetWindow<ConfigWindow>().DrawWarningThing();

    public virtual void OnXDraw() { }
}
