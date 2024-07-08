using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class PetRenameNode : Node
{
    readonly DalamudServices DalamudServices;

    readonly IconNode IconNode;

    IPetSheetData? ActivePet;
    string? CurrentValue = null;


    public Action<string?>? OnSave;

    readonly CircleImageNode CircleImageNode;

    readonly RenameTitleNode SpeciesNode;
    readonly RenameTitleNode RaceNode;
    readonly RenameTitleNode BehaviourNode;
    readonly RenameTitleNode NicknameNode;

    Node? activeHoverNode;

    public PetRenameNode(string? customName, in IPetSheetData? activePet, in DalamudServices services)
    {
        DalamudServices = services;
        CurrentValue = customName;
        ActivePet = activePet;

        ChildNodes = [
            // Rename Node part
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                },
                ChildNodes = [
                    SpeciesNode = new RenameTitleNode("Species:", ActivePet?.BaseSingular ?? "...")
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["MarginSheet"],
                    },
                    RaceNode = new RenameTitleNode("Race:", ActivePet?.RaceName ?? "...")
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["MarginSheet"],
                    },
                    BehaviourNode = new RenameTitleNode("Behaviour:", ActivePet?.BehaviourName ?? "...")
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["MarginSheet"],
                    },
                    NicknameNode = new RenameTitleNode("Nickname:", CurrentValue ?? "...")
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["MarginSheet"],
                    },
                ]
            },

        // This is the node that holds the image together
        new Node()
        {
            //Overflow = false,
            Style = new Style()
            {
                Margin = new EdgeSize(45, 60, 0, 0),
            },
            ChildNodes = [
                    IconNode = new IconNode()
                    {
                        Style = new Style()
                        {
                            Size = new Size(120, 120),
                            IconId = ActivePet?.Icon ?? 66310,
                            BorderColor = new BorderColor(new Color(255, 255, 255)),
                            BorderWidth = new EdgeSize(4),
                            BorderRadius = 18,
                        },
                        ChildNodes = [
                             CircleImageNode = new CircleImageNode(in services)
                             {
                                 Opacity = 0.3f,
                                 RoationSpeed = 12,
                                 Style = new Style()
                                 {
                                     Anchor = Anchor.MiddleCenter,
                                     Size = new Size(190, 190)
                                 }
                             }
                        ]
                    },
                ]
            }
        ];

        SpeciesNode.Hovered += () => activeHoverNode = SpeciesNode;
        RaceNode.Hovered += () => activeHoverNode = RaceNode;
        BehaviourNode.Hovered += () => activeHoverNode = BehaviourNode;
        NicknameNode.Hovered += () => activeHoverNode = NicknameNode;
        IconNode.OnMouseEnter += _ => activeHoverNode = IconNode;

        SpeciesNode.HoveredExit += () => activeHoverNode = null;
        RaceNode.HoveredExit += () => activeHoverNode = null;
        BehaviourNode.HoveredExit += () => activeHoverNode = null;
        NicknameNode.HoveredExit += () => activeHoverNode = null;
        IconNode.OnMouseLeave += _ => activeHoverNode = null;
    }

    public void Setup(string? customName, in IPetSheetData? activePet)
    {
        ActivePet = activePet;
        IconNode.IconID = activePet?.Icon ?? 66310;
        SpeciesNode.SetText(activePet?.BaseSingular ?? "...");
        RaceNode.SetText(activePet?.RaceName ?? "...");
        BehaviourNode.SetText(activePet?.BehaviourName ?? "...");
        NicknameNode.SetText(customName ?? "...");
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        base.OnDraw(drawList);

        if (activeHoverNode != null)
        {
            CircleImageNode.Opacity = 0.8f;
            CircleImageNode.RoationSpeed = 70;
        }
        else 
        {
            CircleImageNode.Opacity = 0.3f;
            CircleImageNode.RoationSpeed = 12;
        }

        Rect activeRect = SpeciesNode.UnderlineNode.Bounds.ContentRect;
        Rect iconRect = IconNode.Bounds.ContentRect;

        Vector2 activePos = activeRect.TopRight + (activeRect.BottomRight - activeRect.TopRight) * 0.5f - new Vector2(2, 0);
        Vector2 iconPos = iconRect.TopLeft + (iconRect.BottomLeft - iconRect.TopLeft) * 0.5f ;
        Vector2 earlyiconPos = iconPos - new Vector2(45, 0);

        drawList.AddLine(activePos, earlyiconPos, new Color(255, 255, 255, 255).ToUInt(), 4);
        drawList.AddLine(earlyiconPos, iconPos, new Color(255, 255, 255, 255).ToUInt(), 4);
    }

    Stylesheet stylesheet = new Stylesheet([
        new(".MarginSheet", new Style()
        {
            Margin = new EdgeSize(0, 0, 0, 15),
        }),
    ]);
}
