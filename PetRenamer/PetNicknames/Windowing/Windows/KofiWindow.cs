using Dalamud.Utility;
using Dalamud.Bindings.ImGui;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Components.Labels;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows;

internal class KofiWindow : PetWindow
{
    protected override Vector2 MinSize     { get; } = new Vector2(350, 136);
    protected override Vector2 MaxSize     { get; } = new Vector2(350, 136);
    protected override Vector2 DefaultSize { get; } = new Vector2(350, 136);

    protected override bool HasModeToggle  { get; } = false;

    public KofiWindow(WindowHandler windowHandler, DalamudServices dalamudServices, Configuration configuration) 
        : base(windowHandler, dalamudServices, configuration, "Pet Nicknames Kofi-Window", ImGuiWindowFlags.None) { }

    protected override void OnDraw()
    {
        BasicLabel.Draw(Translator.GetLine("Kofi.Line1"), WindowHandler.StretchingBar);
        BasicLabel.Draw(Translator.GetLine("Kofi.Line2"), WindowHandler.StretchingBar);

        float width = 100 * WindowHandler.GlobalScale;

        ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2(ImGui.GetContentRegionAvail().X * 0.5f - width * 0.5f, 0));

        if (ImGui.Button(Translator.GetLine("Kofi.TakeMe") + "##Kofi_{WindowHandler.InternalCounter}", new Vector2(width, WindowHandler.BarHeight)))
        {
            Util.OpenLink("https://ko-fi.com/glyceri");
        }
    }
}
