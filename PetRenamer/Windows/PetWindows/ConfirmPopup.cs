using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using System;

namespace PetRenamer.Windows.PetWindows;

public class ConfirmPopup : TemporaryPetWindow
{
    string message;
    Window blackenedWindow;

    public ConfirmPopup(string message, Action<object> callback, Window blackenedWindow = null!) : base(message, callback, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoTitleBar)
    {
        Size = new Vector2(290, 140);
        SizeCondition = ImGuiCond.Always;
        if (blackenedWindow != null)
            Position = blackenedWindow.Position;
        this.message = message;
        this.blackenedWindow = blackenedWindow!;
        IsOpen = true;
    }

    public override void OnDraw()
    {
        if (blackenedWindow != null) blackenedWindow.IsOpen = false;
        ImGui.TextColored(StylingColours.errorText, message);

        bool? outcome = null;

        
        if (Button("Yes")) outcome = true;
        ImGui.SameLine();
        if (Button("No")) outcome = false;
        if (outcome == null) return;

        IsOpen = false;
        if (blackenedWindow != null) blackenedWindow.IsOpen = true;
        DoCallback(outcome.Value);
    }
}
