using ImGuiNET;
using PetRenamer.PetNicknames.ColourProfiling;
using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using System;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;

internal class ColourSettingsNode : Node
{
    readonly Configuration Configuration;
    readonly string Label;

    readonly Node UnderlineNode;
    readonly Node LabelNode;
    readonly Node buttonClick;

    uint currentValue;
    IColourProfile? myProfile = null;

    readonly Action OnEdit;

    public ColourSettingsNode(in Configuration configuration, string label, Action onEdit)
    {
        Configuration = configuration;
        currentValue = new Color(label).ToUInt();
        Label = label;
        OnEdit = onEdit;

        Style = new Style()
        {
            Flow = Flow.Horizontal,
        };

        ChildNodes = [
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Size = new Size(15, 18),
                    Gap = 1,
                },
                ChildNodes = [
                    buttonClick = new Node()
                    {
                        NodeValue = string.Empty,
                        Style = new Style()
                        {
                            Flow = Flow.Vertical,
                            Size = new Size(15, 15),
                            BackgroundColor = new Color(currentValue),
                        },
                    },
                    UnderlineNode = new Node()
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["UnderlineNode"],
                    },
                ]
            },
            LabelNode = new Node()
            {
                Stylesheet = stylesheet,
                ClassList = ["LabelNode"],
                NodeValue = Translator.GetLine("ColourSetting." + label),
            },
        ];
    }

    public void UpdateProfile(IColourProfile colourProfile) 
    {
        myProfile = colourProfile;

        
        Set();
    }

    void Set()
    {
        PetColour? colour = myProfile?.GetColour(Label);
        if (colour == null) return;

        currentValue = colour.Colour;
    }

    void OnColourChange()
    {
        myProfile?.SetColor(Label, currentValue);
        OnEdit?.Invoke();
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        Vector4 value = ImGui.ColorConvertU32ToFloat4(new Color(currentValue).ToUInt());

        buttonClick.Style.BackgroundColor = new(currentValue);

        var bounds = buttonClick.Bounds.PaddingRect;
        var popupId = $"##{LabelNode.NodeValue}";
        var pickerId = $"##{LabelNode.NodeValue}";

        ImGui.SetCursorScreenPos(bounds.TopLeft);

        ImGui.PushID($"ColorPicker{LabelNode.NodeValue}");

        if (ImGui.InvisibleButton($"##{LabelNode.NodeValue}", new(bounds.Width, bounds.Height)))
        {
            ImGui.OpenPopup(popupId);
        }

        if (ImGui.BeginPopup(popupId))
        {
            ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 1);
            ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 8);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(4, 4));
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(4, 4));

            if (ImGui.ColorPicker4(
                    pickerId,
                    ref value,
                    ImGuiColorEditFlags.NoLabel
                    | ImGuiColorEditFlags.AlphaBar
                    | ImGuiColorEditFlags.DisplayMask
                    | ImGuiColorEditFlags.AlphaPreview
                    | ImGuiColorEditFlags.NoSidePreview
                    | ImGuiColorEditFlags.NoSmallPreview
                    | ImGuiColorEditFlags.NoTooltip
                    
                ))
            {
                currentValue = ImGui.ColorConvertFloat4ToU32(value);
                OnColourChange();
            }

            ImGui.EndPopup();
            ImGui.PopStyleVar(4);
        }

        ImGui.PopID();
    }

    readonly Stylesheet stylesheet = new Stylesheet([
        new(".LabelNode", new Style()
        {
            Size = new Size(270, 15),
            TextAlign = Anchor.TopLeft,
            TextOffset = new System.Numerics.Vector2(0, 3),
            FontSize = 8,
            TextOverflow = false,
            Color = new Color("Window.TextLight"),
            OutlineColor = new("Window.TextOutline"),
            OutlineSize = 1,
        }
        ),
        new(".UnderlineNode", new Style()
        {
            Size = new Size(300, 2),
            BackgroundGradient = GradientColor.Horizontal(new Color("Outline"), new Color("Outline:Fade")),
            RoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
            BorderRadius = 3,
        }),
    ]
    );
}
