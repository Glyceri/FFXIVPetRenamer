using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetRenamer.Windows
{
    public class ConfirmPopup : Window
    {
        Action<bool> callback;
        string message;
        Window blackenedWindow;

        public ConfirmPopup(string message, Action<bool> callback, Window blackenedWindow = null) : base(message, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.Size = new Vector2(400, 110);
            this.SizeCondition = ImGuiCond.Always;
            if(blackenedWindow != null)
                this.Position = blackenedWindow.Position;
            this.callback = callback;
            this.message = message;
            this.blackenedWindow = blackenedWindow;
            IsOpen = true;

           // PluginLink.PetRenamerPlugin.WindowSystem.AddWindow(this);
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
            //PluginLink.PetRenamerPlugin.WindowSystem.RemoveWindow(this);
            if (blackenedWindow != null) blackenedWindow.IsOpen = true;
            callback?.Invoke(outcome.Value);
        }
    }
}
