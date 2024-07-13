using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base.Style;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.Windowing.Enums;
using PetRenamer.PetNicknames.Windowing.Interfaces;
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

    protected abstract string Title { get; }

    protected readonly DalamudServices DalamudServices;

    readonly BackgroundNode _windowNode;

    readonly Node TitlebarNode;
    readonly Node TitlebarTextNode;
    protected readonly Node ContentNode;
    readonly QuickSquareButton CloseButton;

    Vector2 lastSize = Vector2.Zero;

    protected PetWindow(DalamudServices dalamudServices, string name, ImGuiWindowFlags additionalFlags = ImGuiWindowFlags.None) : base(name, ImGuiWindowFlags | additionalFlags, true)
    {
        DalamudServices = dalamudServices;

        _windowNode = new BackgroundNode(194019u)
        {
            Stylesheet = WindowStyles.WindowStylesheet,
            ClassList = ["window"],
            ChildNodes =
            [
                TitlebarNode = new Node()
                {
                    ClassList = ["window--titlebar"],
                },
                TitlebarTextNode = new Node()
                {
                    ClassList = ["window--titlebar-text"],
                    NodeValue = Title,
                },
                CloseButton = new QuickSquareButton()
                {
                    NodeValue = FontAwesomeIcon.Times.ToIconString(),
                    Style = new Una.Drawing.Style()
                    {
                        Anchor = Anchor.TopRight,
                        Size = new Size(20, 20),
                        FontSize = 12,
                        Margin = new EdgeSize(6),
                    }
                },
                ContentNode = new Node()
                {
                    ClassList = ["window--content"],
                },
            ]
        };

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = MinSize,
            MaximumSize = MaxSize,
        };
        Size = DefaultSize;
        SizeCondition = ImGuiCond.FirstUseEver;

        CloseButton.OnClick += () => DalamudServices.Framework.Run(Close);

        if (HasModeToggle) PetModeConstructor();
    }

    public void Close() => IsOpen = false;
    public void Open() => IsOpen = true;

    public sealed override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0);
    }

    public sealed override void PostDraw()
    {
        ImGui.PopStyleVar(6);
    }

    public sealed override void Draw()
    {
        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Right))
        {
            Node.DrawDebugInfo ^= true;
        }

        Vector2 size = ImGui.GetWindowSize() * (1.0f / Node.ScaleFactor);
        if (lastSize != size)
        {
            lastSize = size;

            _windowNode.Style.Size = new Size((int)size.X - 3, (int)size.Y - 3);

            TitlebarNode.Style.Size = new((int)size.X - 9, 32);
            TitlebarTextNode.Style.Size = new((int)size.X - 9, 32);

            ContentNode.Style.Size = new((int)size.X - 9, (int)size.Y - 41);
            ContentSize = new(ContentNode.Style.Size.Width, ContentNode.Style.Size.Height);
        }
        _windowNode.Style.StrokeColor = IsFocused ? WindowStyles.WindowBorderActive : WindowStyles.WindowBorderInactive;

        RenderWindowInstance();
    }

    void RenderWindowInstance()
    {
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        ImGui.SetCursorPos(new(0, 0));

        OnDraw();

        Vector2 ps = ImGui.GetWindowPos();
        Point pt = new((int)ps.X + 2, (int)ps.Y + 2);
        _windowNode.Render(drawList, pt);

        OnLateDraw();
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

    public abstract void OnDraw();
    public virtual void OnLateDraw() { }
    public virtual void OnDirty() { }

    static ImGuiWindowFlags ImGuiWindowFlags =>
         ImGuiWindowFlags.NoTitleBar |
         ImGuiWindowFlags.NoCollapse |
         ImGuiWindowFlags.NoDocking |
         ImGuiWindowFlags.NoScrollbar |
         ImGuiWindowFlags.NoScrollWithMouse |
         ImGuiWindowFlags.NoBackground |
        ImGuiWindowFlags.None;
}
