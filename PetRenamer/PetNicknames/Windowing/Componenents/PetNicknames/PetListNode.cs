using Dalamud.Interface;
using ImGuiNET;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class PetListNode : Node
{
    readonly IconNode IconNode;
    readonly Node ClearButtonNode;

    readonly RenameTitleNode SpeciesNode;
    readonly RenameTitleNode IDNode;
    readonly NicknameEditNode NicknameNode;

    public PetListNode(in IPetSheetData data, string? customName)
    {
        Style = new Style()
        {
            Flow = Flow.Horizontal,
            BackgroundColor = new Color(26, 20, 0, 130),
            Size = new Size(412, 70),
            RoundedCorners = RoundedCorners.All,
            BorderRadius = 8,
            StrokeRadius = 8,
            BorderColor = new(new("Window.TitlebarBorder")),
            BorderWidth = new EdgeSize(1),
            IsAntialiased = false,
        };

        ChildNodes = [
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Margin = new EdgeSize(6, 0, 0, 8),
                },
                ChildNodes = [
                    SpeciesNode = new RenameTitleNode($"{Translator.GetLine("PetRenameNode.Species")}:", data.BaseSingular),
                    IDNode = new RenameTitleNode($"ID:", data.Model.ToString()),
                    NicknameNode = new NicknameEditNode($"{Translator.GetLine("PetRenameNode.Nickname")}:", customName ?? "..."),
                ]
            },
            IconNode = new IconNode()
            {
                Style = new Style()
                {
                    Size = new Size(60, 60),
                    Anchor = Anchor.MiddleRight,
                    Margin = new EdgeSize(0, 26, 0, 0),
                    BorderColor = new BorderColor(new Color(255, 255, 255)),
                    BorderWidth = new EdgeSize(4),
                    BorderRadius = 6,
                }
            },
            ClearButtonNode = new Node()
            {
                NodeValue = FontAwesomeIcon.Times.ToIconString(),
                Stylesheet = stylesheet,
                ClassList = ["ClearButton"],
            }
        ];

        NicknameNode.SetPet(customName, data);
        ClearButtonNode.OnClick += _ => { };

        IconNode.IconID = data.Icon;
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        base.OnDraw(drawList);

        Rect activeRect = SpeciesNode.UnderlineNode.Bounds.ContentRect;
        Rect iconRect = IconNode.Bounds.ContentRect;

        Vector2 activePos = activeRect.TopRight + (activeRect.BottomRight - activeRect.TopRight) * 0.5f - new Vector2(Node.ScaleFactor, 0);
        Vector2 iconPos = iconRect.TopLeft + (iconRect.BottomLeft - iconRect.TopLeft) * 0.5f;
        Vector2 earlyiconPos = iconPos - new Vector2(12, 0) * Node.ScaleFactor;

        drawList.AddLine(activePos, earlyiconPos + new Vector2(Node.ScaleFactor * 0.5f, 0), new Color(255, 255, 255, 255).ToUInt(), 2 * Node.ScaleFactor);
        drawList.AddLine(earlyiconPos, iconPos, new Color(255, 255, 255, 255).ToUInt(), 2 * Node.ScaleFactor);
    }

    Stylesheet stylesheet = new Stylesheet([
        new (".ClearButton", new Style()
        {
            Anchor = Anchor.TopRight,
            Size = new(15, 15),
            BackgroundColor = new("Window.BackgroundLight"),
            StrokeColor = new("Window.TitlebarBorder"),
            StrokeWidth = 1,
            StrokeInset = 0,
            BorderRadius = 3,
            TextAlign = Anchor.MiddleCenter,
            Font = 2,
            FontSize = 10,
            Color = new("Window.TextLight"),
            OutlineColor = new("Window.TextOutline"),
            TextOverflow = true,
            Margin = new() { Top = 6, Right = 6 },
            IsAntialiased = false,
        }),
        new(".ClearButton:hover", new Style()
        {
            BackgroundColor = new("Window.Background"),
            StrokeWidth = 2,
        })
    ]);
}
