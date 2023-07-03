using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using ImGuiScene;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;
using System;
using System.IO;

namespace PetRenamer.Windows.PetWindows
{
    [PersistentPetWindow]
    public class CreditsWindow : PetWindow, IDisposable
    {
        TextureWrap bruno;

        public CreditsWindow() : base(
       "Credits",
       ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
       ImGuiWindowFlags.NoScrollWithMouse)
        {
            Size = new Vector2(524, 612);
            SizeCondition = ImGuiCond.Always;

            var brunoPath = Path.Combine(PluginHandlers.PluginInterface.AssemblyLocation.Directory?.FullName!, "Bruno.png");

            bruno = PluginHandlers.PluginInterface.UiBuilder.LoadImage(brunoPath);
        }

        protected override void OnDispose()
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
