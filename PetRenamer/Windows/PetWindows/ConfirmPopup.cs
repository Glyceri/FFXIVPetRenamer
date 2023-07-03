using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using System;

namespace PetRenamer.Windows.PetWindows
{
    public class ConfirmPopup : TemporaryPetWindow
    {
        string message;
        Window blackenedWindow;

        public ConfirmPopup(string message, Action<object> callback, Window blackenedWindow = null!) : base(message, callback, blackenedWindow,ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            Size = new Vector2(400, 110);
            SizeCondition = ImGuiCond.Always;
            if (blackenedWindow != null)
                Position = blackenedWindow.Position;
            this.message = message;
            this.blackenedWindow = blackenedWindow!;
            IsOpen = true;
        }

        public override void Draw()
        {
            if (blackenedWindow != null) blackenedWindow.IsOpen = false;
            ImGui.TextColored(new Vector4(1, 0, 0, 1), message);

            bool? outcome = null;

            if (ImGui.Button("Yes")) outcome = true;
            if (ImGui.Button("No")) outcome = false;
            if (outcome == null) return;

            IsOpen = false;
            if (blackenedWindow != null) blackenedWindow.IsOpen = true;
            DoCallback(outcome.Value);
        }
    }
}
