using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.AutoRegistry.Interfaces;
using PetRenamer.Core.Handlers;

namespace PetRenamer.Windows;

public abstract class PetWindow : Window, IDisposableRegistryElement
{ 
    protected PetWindow(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow) { }

    public void Dispose() => OnDispose();
    protected virtual void OnDispose() { }

    public sealed override unsafe void Draw()
    {
        PushStyleColor(ImGuiCol.TitleBgActive, StylingColours.titleBgActive);
        PushStyleColor(ImGuiCol.TitleBg, StylingColours.titleBg);
        PushStyleColor(ImGuiCol.TitleBgCollapsed, StylingColours.tileBgCollapsed);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        OnDraw();
        PopAllStyleColours();
    }

    public unsafe virtual void OnDraw() { }

    protected static class Styling
    {
        public static Vector2 ListButton = new Vector2(150, 25);
        public static Vector2 ListNameButton = new Vector2(480, 25);
        public static Vector2 ListIDField = new Vector2(75, 25);
        public static Vector2 SmallButton = new Vector2(25, 25);

    }

    protected static class StylingColours
    {
        public static Vector4 defaultBackground         = new Vector4(0.60f, 0.70f, 0.80f, 0.95f);
        public static Vector4 titleBgActive             = new Vector4(0.30f, 0.50f, 1f, 1f);
        public static Vector4 titleBg                   = new Vector4(0.20f, 0.30f, 0.6f, 1f);
        public static Vector4 tileBgCollapsed           = new Vector4(0.1f, 0.1f, 0.1f, 1f);
        public static Vector4 defaultText               = new Vector4(0.95f, 0.95f, 0.95f, 1f);

        public static Vector4 idleColor                 = new Vector4(0.4f, 0.4f, 0.5f, 1f);

        public static Vector4 ButtonHovered             = new Vector4(0.5f, 0.5f, 1f, 1f);
        public static Vector4 ButtonPressed             = new Vector4(0.36f, 0.36f, 1f, 1f);
        public static Vector4 Button                    = new Vector4(0.3f, 0.3f, 1f, 1f);

        public static Vector4 XButtonHovered            = new Vector4(0.5f, 0.5f, 0.7f, 1f);
        public static Vector4 XButtonPressed            = new Vector4(0.36f, 0.36f, 0.7f, 1f);
        public static Vector4 XButton                   = new Vector4(0.3f, 0.3f, 0.7f, 1f);

        public static Vector4 listBox                   = new Vector4(0.26f, 0.26f, 0.38f, 1f);
    }

    protected bool Button(string text)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.ButtonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.Button);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.ButtonPressed);
        return ImGui.Button(text);
    }

    protected bool Button(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.ButtonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.Button);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.ButtonPressed);
        return ImGui.Button(text, styling);
    }

    protected bool XButton(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.XButtonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.XButton);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.XButtonPressed);
        return ImGui.Button(text, styling);
    }

    protected void Label(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered,    StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Button,           StylingColours.idleColor);
        PushStyleColor(ImGuiCol.ButtonActive,     StylingColours.idleColor);
        ImGui.Button(text, styling);
    }

    protected bool Checkbox(string text, ref bool value)
    {
        PushStyleColor(ImGuiCol.CheckMark, StylingColours.defaultText);
        PushStyleColor(ImGuiCol.FrameBgHovered, StylingColours.XButtonHovered);
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.XButton);
        PushStyleColor(ImGuiCol.FrameBgActive, StylingColours.XButtonPressed);
        return ImGui.Checkbox(text, ref value);
    }


    int popCount = 0;
    protected void PushStyleColor(ImGuiCol imGuiCol, Vector4 colour)
    {
        if (!PluginLink.Configuration.useCustomTheme) return;
        ImGui.PushStyleColor(imGuiCol, colour);
        popCount++;
    }

    protected void PopAllStyleColours() 
    {
        ImGui.PopStyleColor(popCount);
        popCount = 0;
    }

    protected bool BeginListBox(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.listBox);
        return ImGui.BeginListBox(text, styling);
    }
}
