using Dalamud.Interface.Windowing;
using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Enums;
using PetRenamer.PetNicknames.Windowing.Interfaces;
using System;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Base;

internal abstract partial class PetWindow : Window, IPetWindow
{
    protected PetWindowMode CurrentMode { get; private set; }

    protected abstract string ID { get; }
    protected abstract Vector2 MinSize { get; }
    protected abstract Vector2 MaxSize { get; }
    protected abstract Vector2 DefaultSize { get; }

    protected abstract bool HasModeToggle { get; }

    protected Size ContentSize { get; private set; } = new Size();

    public bool IsFocused { get; private set; }
    public bool IsHovered { get; private set; }

    protected abstract Node Node { get; }

    protected abstract string Title { get; }

    protected readonly DalamudServices DalamudServices;

    protected PetWindow(DalamudServices dalamudServices, string name) : base(name, ImGuiWindowFlags, true)
    {
        DalamudServices = dalamudServices;
        CloseButton.OnClick += _ => Close();

        ContentNode.AppendChild(Node);

        if (HasModeToggle) PetModeConstructor();
    }

    public void Close()
    {
        IsOpen = false;
    }

    public void Open()
    {
        IsOpen = true;
    }

    public sealed override void Draw()
    {
        ImGui.SetNextWindowViewport(ImGui.GetMainViewport().ID);
        ImGui.SetNextWindowSizeConstraints(MinSize * Node.ScaleFactor, MaxSize * Node.ScaleFactor);
        ImGui.SetNextWindowSize(DefaultSize, ImGuiCond.FirstUseEver);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);

        if (ImGui.Begin($"{ID}", ImGuiWindowFlags))
        {
            IsFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows);
            IsHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows);

            Vector2 size = ImGui.GetWindowSize() / Node.ScaleFactor;
            size.X = (float)Math.Floor(size.X);
            size.Y = (float)Math.Floor(size.Y);

            _windowNode.Style.Size = new Size((int)size.X - 2, (int)size.Y - 2);

            TitlebarNode.Style.Size = new((int)size.X - 7, 32);
            TitlebarTextNode.Style.Size = new((int)size.X - 64, 32);

            ContentNode.Style.Size = new((int)size.X - 7, (int)size.Y - 39);
            Node.Style.Margin = new(1);
            ContentSize = new(ContentNode.Style.Size.Width - 2, ContentNode.Style.Size.Height - 2);
            Node.Style.Size = ContentSize;

            // Only enable shadow if the window has focus.
            _windowNode.Style.ShadowSize = IsFocused ? new(64) : new(0);
            _windowNode.Style.StrokeColor = IsFocused ? new Color("Window.Border:Active") : new Color("Window.Border:Inactive");

            TitlebarNode.QuerySelector("TitleText")!.NodeValue = Title;

            RenderWindowInstance(ID);

            ImGui.End();
        }

        ImGui.PopStyleVar(6);
    }

    void RenderWindowInstance(string id, int instanceId = 0)
    {
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        ImGui.SetCursorPos(new(0, 0));
        ImGui.BeginChild($"PetWindow_{id}##{instanceId}", ImGui.GetWindowSize(), false);

        // Here to ensure stylevars get popped
        try { OnDaw(); } catch (Exception ex) { DalamudServices.PluginLog.Error(ex.Message); }

        Vector2 ps = ImGui.GetWindowPos();
        Point pt = new((int)ps.X + 2, (int)ps.Y + 2);

        // Here to ensure stylevars get popped
        try { _windowNode.Render(drawList, pt); } catch(Exception ex) { DalamudServices.PluginLog.Error(ex.Message); }

        ImGui.EndChild();
    }

    protected void AddNode(Node parentNode, Node newNode)
    {
        DalamudServices.Framework.Run(() => parentNode.AppendChild(newNode));
    }
    
    protected void PrepependNode(Node parentNode, Node newNode)
    {
        DalamudServices.Framework.Run(() => parentNode.ChildNodes.Insert(0, newNode));
    }

    protected void RemoveNode(Node parentNode, Node oldNode)
    {
        DalamudServices.Framework.Run(() => parentNode.RemoveChild(oldNode));
    }

    Node TitlebarNode => _windowNode.QuerySelector(".window--titlebar")!;
    Node TitlebarTextNode => _windowNode.QuerySelector(".window--titlebar-text")!;
    Node ContentNode => _windowNode.QuerySelector(".window--content")!;
    Node TopLeftAnchor => TitlebarNode.QuerySelector("#TitleLeftAnchor")!;
    Node MiddleAnchor => TitlebarNode.QuerySelector("#TitleMiddleAnchor")!;
    Node TopRightAnchor => TitlebarNode.QuerySelector("#TitleRightAnchor")!;

    protected Node CloseButton => _windowNode.QuerySelector("CloseButton")!;

    public abstract void OnDaw();

    static ImGuiWindowFlags ImGuiWindowFlags =>
        ImGuiWindowFlags.NoTitleBar
        | ImGuiWindowFlags.NoCollapse
        | ImGuiWindowFlags.NoDocking
        | ImGuiWindowFlags.NoScrollbar
        | ImGuiWindowFlags.NoScrollWithMouse
        | ImGuiWindowFlags.NoBackground;


}
