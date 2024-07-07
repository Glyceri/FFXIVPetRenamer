using System;
using System.Numerics;
using ImGuiNET;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents;

internal class StringInputNode : Node
{
    public event Action<string>? OnValueChanged;

    public string Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public uint MaxLength { get; set; }

    private string _value;
    private bool _immediate;

    public StringInputNode(
        string id,
        string value,
        uint maxLength,
        bool immediate = false
    )
    {
        _value = value;
        _immediate = immediate;
        MaxLength = maxLength;

        Id = id;
        ClassList = ["input"];
        Stylesheet = InputStylesheet;

        ChildNodes = [
            new()
            {
                ClassList = ["input--box"],
            },
        ];

        BeforeReflow += _ =>
        {
            SelectBoxNode.Style.Size = (SelectBoxNode.ParentNode!.Bounds.ContentSize - SelectBoxNode.ParentNode!.ComputedStyle.Margin.Size * 2) / ScaleFactor;
            SelectBoxNode.Style.Margin = SelectBoxNode.ParentNode!.ComputedStyle.Margin;

            return true;
        };
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        var bounds = QuerySelector(".input--box")!.Bounds;
        ImGui.SetCursorScreenPos(bounds.ContentRect.TopLeft);

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(8, 4));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(8, 4));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 5);

        ImGui.PushStyleColor(ImGuiCol.Text, new Color("Input.Text").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.PopupBg, new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.Button, new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new Color("Input.Background").ToUInt());
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive, new Color("Input.Background").ToUInt());

        ImGui.SetNextItemWidth(bounds.ContentSize.Width);

        if (ImGui.InputText($"##{Id}", ref _value, MaxLength, !_immediate ? ImGuiInputTextFlags.EnterReturnsTrue : ImGuiInputTextFlags.None))
        {
            OnValueChanged?.Invoke(_value);
        }

        if (ImGui.IsItemDeactivatedAfterEdit())
        {
            OnValueChanged?.Invoke(_value);
        }

        ImGui.PopStyleVar(4);
        ImGui.PopStyleColor(7);
    }

    private Node SelectBoxNode => QuerySelector(".input--box")!;

    private static Stylesheet InputStylesheet { get; } = new(
        [
            new(
                ".input",
                new()
                {
                    Flow = Flow.Vertical,
                    Gap = 6,
                    Padding = new() { Left = 32 }
                }
            ),
            new(
                ".input--box",
                new()
                {
                    Size = new(0, 26),
                }
            ),
            new(
                ".input--label",
                new()
                {
                    FontSize = 14,
                    Color = new("Window.Text"),
                    TextOverflow = false,
                    WordWrap = false,
                }
            ),
            new(
                ".input--description",
                new()
                {
                    FontSize = 11,
                    Color = new("Window.TextMuted"),
                    TextOverflow = false,
                    WordWrap = true,
                    LineHeight = 1.5f,
                }
            ),
        ]
    );
}
