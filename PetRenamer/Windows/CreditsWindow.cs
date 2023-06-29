using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using ImGuiScene;
using System;
using System.IO;

namespace PetRenamer.Windows
{
    public class CreditsWindow : Window, IDisposable
    {
        TextureWrap bruno;

        public CreditsWindow(PetRenamerPlugin plugin) : base(
       "Credits",
       ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
       ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.Size = new Vector2(512, 612);
            this.SizeCondition = ImGuiCond.Always;

            string brunoPath = Path.Combine(plugin.PluginInterface.AssemblyLocation.Directory?.FullName!, "Bruno.png");

            bruno = plugin.PluginInterface.UiBuilder.LoadImage(brunoPath);
        }

        public void Dispose()
        {
            bruno.Dispose();
        }

        public override void Draw()
        {
            ImGui.TextColored(new Vector4(0.6f, 1, 1, 1), "Created by: Glyceri");
            ImGui.TextColored(new Vector4(0.6f, 1, 1, 1), "In loving memory of: Bruno");
            ImGui.Image(bruno.ImGuiHandle, new Vector2(512, 512));
        }
    }
}
