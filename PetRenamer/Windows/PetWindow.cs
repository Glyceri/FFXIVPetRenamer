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
        PushStyleColor(ImGuiCol.ScrollbarGrab, StylingColours.button);
        PushStyleColor(ImGuiCol.ScrollbarGrabActive, StylingColours.buttonPressed);
        PushStyleColor(ImGuiCol.ScrollbarGrabHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.ScrollbarBg, StylingColours.scrollBarBG);
        OnDraw();
        PopAllStyleColours();
    }

    public unsafe virtual void OnDraw() { }

    public static class Styling
    {
        public static Vector2 ListButton = new Vector2(150, 25);
        public static Vector2 ListNameButton = new Vector2(480, 25);
        public static Vector2 ListIDField = new Vector2(75, 25);
        public static Vector2 SmallButton = new Vector2(25, 25);

    }

    public static class StylingColours
    {
        public static Vector4 defaultBackground         = new Vector4(0.60f, 0.70f, 0.80f, 0.95f);
        public static Vector4 titleBgActive             = new Vector4(0.30f, 0.50f, 1f, 1f);
        public static Vector4 titleBg                   = new Vector4(0.20f, 0.30f, 0.6f, 1f);
        public static Vector4 tileBgCollapsed           = new Vector4(0.1f, 0.1f, 0.1f, 1f);

        public static Vector4 defaultText               = new Vector4(0.95f, 0.95f, 0.95f, 1f);
        public static Vector4 errorText                 = new Vector4(1, 0, 0, 1.0f);
        public static Vector4 blueText                  = new Vector4(0.6f, 0.6f, 1f, 1f);
        public static Vector4 readableBlueText          = new Vector4(0.8f, 0.8f, 1f, 1f);

        public static Vector4 idleColor                 = new Vector4(0.4f, 0.4f, 0.5f, 1f);

        public static Vector4 buttonHovered             = new Vector4(0.5f, 0.5f, 1f, 1f);
        public static Vector4 buttonPressed             = new Vector4(0.36f, 0.36f, 1f, 1f);
        public static Vector4 button                    = new Vector4(0.3f, 0.3f, 1f, 1f);

        public static Vector4 textFieldHovered          = new Vector4(0.5f, 0.5f, 1f, 1f);
        public static Vector4 textFieldPressed          = new Vector4(0.36f, 0.36f, 1f, 1f);
        public static Vector4 textField                 = new Vector4(0.3f, 0.3f, 0.8f, 1f);

        public static Vector4 xButtonHovered            = new Vector4(0.5f, 0.5f, 0.8f, 1f);
        public static Vector4 xButtonPressed            = new Vector4(0.36f, 0.36f, 0.8f, 1f);
        public static Vector4 xButton                   = new Vector4(0.3f, 0.3f, 0.8f, 1f);

        public static Vector4 listBox                   = new Vector4(0.26f, 0.26f, 0.36f, 1f);
        public static Vector4 scrollBarBG               = new Vector4(0.29f, 0.29f, 0.36f, 1f);
    }

    protected bool Button(string text)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.button);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.buttonPressed);
        return ImGui.Button(text);
    }

    protected bool Button(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.buttonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.button);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.buttonPressed);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        return ImGui.Button(text, styling);
    }

    protected bool XButton(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.xButtonHovered);
        PushStyleColor(ImGuiCol.Button, StylingColours.xButton);
        PushStyleColor(ImGuiCol.ButtonActive, StylingColours.xButtonPressed);
        PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
        return ImGui.Button(text, styling);
    }

    protected void Label(string text, Vector2 styling)
    {
        PushStyleColor(ImGuiCol.ButtonHovered,    StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Button,           StylingColours.idleColor);
        PushStyleColor(ImGuiCol.ButtonActive,     StylingColours.idleColor);
        PushStyleColor(ImGuiCol.Text, StylingColours.readableBlueText);      
        ImGui.Button(text, styling);
    }

    protected bool Checkbox(string text, ref bool value)
    {
        PushStyleColor(ImGuiCol.CheckMark, StylingColours.defaultText);
        PushStyleColor(ImGuiCol.FrameBgHovered, StylingColours.xButtonHovered);
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.xButton);
        PushStyleColor(ImGuiCol.FrameBgActive, StylingColours.xButtonPressed);
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

    protected bool InputText(string label, ref string input, uint maxLength, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        PushStyleColor(ImGuiCol.FrameBg, StylingColours.textField);
        PushStyleColor(ImGuiCol.FrameBgActive, StylingColours.textFieldPressed);
        PushStyleColor(ImGuiCol.FrameBgHovered, StylingColours.textFieldHovered);
        return ImGui.InputText(label, ref input, maxLength, flags);
    }

    protected void SameLine() => ImGui.SameLine();
    protected void SameLineNoMargin() => ImGui.SameLine(0, 0.0000001f);
    protected void SameLinePretendSpace() => ImGui.SameLine(0, 3f);
}
