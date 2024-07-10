using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
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
    readonly NicknameEditNode NicknameNode;
    readonly RenameTitleNode IDNode;

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
                    SpeciesNode = new RenameTitleNode($"{Translator.GetLine("PetRenameNode.Species")}:", ActivePet?.BaseSingular ?? "...")
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["MarginSheet"],
                    },
                    IDNode = new RenameTitleNode("ID:", ActivePet?.Model.ToString() ?? "...")
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["MarginSheet"],
                    },
                    RaceNode = new RenameTitleNode($"{Translator.GetLine("PetRenameNode.Race")}:", ActivePet?.RaceName ?? "...")
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["MarginSheet"],
                    },
                    BehaviourNode = new RenameTitleNode($"{Translator.GetLine("PetRenameNode.Behaviour")}:", ActivePet?.BehaviourName ?? "...")
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["MarginSheet"],
                    },
                    NicknameNode = new NicknameEditNode($"{Translator.GetLine("PetRenameNode.Nickname")}:", CurrentValue ?? "...")
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
                    Margin = new EdgeSize(19, 0, 0, 30),
                },
                ChildNodes = [
                    IconNode = new IconNode()
                    {
                        Style = new Style()
                        {
                            Size = new Size(90, 90),
                            
                            IconId = ActivePet?.Icon ?? 66310,
                            BorderColor = new BorderColor(new Color(255, 255, 255)),
                            BorderWidth = new EdgeSize(4),
                            BorderRadius = 8,
                        },
                        ChildNodes = [
                             CircleImageNode = new CircleImageNode(in services)
                             {
                                 Opacity = 0.3f,
                                 RoationSpeed = 12,
                                 Style = new Style()
                                 {
                                     Anchor = Anchor.MiddleCenter,
                                     Size = new Size(130, 130)
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
        IDNode.Hovered += () => activeHoverNode = IDNode;

        SpeciesNode.HoveredExit += () => activeHoverNode = null;
        RaceNode.HoveredExit += () => activeHoverNode = null;
        BehaviourNode.HoveredExit += () => activeHoverNode = null;
        NicknameNode.HoveredExit += () => activeHoverNode = null;
        IconNode.OnMouseLeave += _ => activeHoverNode = null;
        IDNode.HoveredExit += () => activeHoverNode = null;

        NicknameNode.OnSave += (value) => OnSave?.Invoke(value);
    }

    public void Setup(string? customName, in IPetSheetData? activePet)
    {
        ActivePet = activePet;
        CurrentValue = customName;
        IconNode.IconID = activePet?.Icon ?? 66310;
        SpeciesNode.SetText(activePet?.BaseSingular ?? "...");
        RaceNode.SetText(activePet?.RaceName ?? "...");
        BehaviourNode.SetText(activePet?.BehaviourName ?? "...");
        IDNode.SetText(ActivePet?.Model.ToString() ?? "...");
        NicknameNode.SetPet(customName, activePet);

        CircleImageNode.Style.IsVisible = activePet != null;
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

        Vector2 activePos = activeRect.TopRight + (activeRect.BottomRight - activeRect.TopRight) * 0.5f - new Vector2(Node.ScaleFactor, 0);
        Vector2 iconPos = iconRect.TopLeft + (iconRect.BottomLeft - iconRect.TopLeft) * 0.5f;
        Vector2 earlyiconPos = iconPos - new Vector2(20.5f, 0) * Node.ScaleFactor;

        drawList.AddLine(activePos, earlyiconPos, new Color(255, 255, 255, 255).ToUInt(), 2 * Node.ScaleFactor);
        drawList.AddLine(earlyiconPos, iconPos, new Color(255, 255, 255, 255).ToUInt(), 2 * Node.ScaleFactor);
    }

    Stylesheet stylesheet = new Stylesheet([
        new(".MarginSheet", new Style()
        {
            Margin = new EdgeSize(2, 0, 0, 15),
        }),
    ]);
}
