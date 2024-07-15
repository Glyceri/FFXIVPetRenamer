using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Images;
using System;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.WindowNodes;

internal class PetRenameNode : Node
{
    readonly DalamudServices DalamudServices;

    readonly IconNode IconNode;

    IPetSheetData? ActivePet;
    string? CurrentValue = null;

    public Action<string?>? OnSave;

    readonly TechnoCircleImageNode CircleImageNode;

    readonly RenameTitleNode SpeciesNode;
    readonly RenameTitleNode RaceNode;
    readonly RenameTitleNode BehaviourNode;
    readonly NicknameEditNode NicknameNode;
    readonly RenameTitleNode IDNode;

    readonly Node HolderNode;

    public PetRenameNode(string? customName, in IPetSheetData? activePet, in DalamudServices services)
    {
        DalamudServices = services;
        CurrentValue = customName;
        ActivePet = activePet;
        ChildNodes =
            [
                // Rename Node part
                HolderNode = new Node()
                {
                    Style = new Style()
                    {
                        Flow = Flow.Vertical,
                        Margin = new EdgeSize(10, 0, 0, 0),
                    },
                    ChildNodes =
                    [
                        SpeciesNode = new RenameTitleNode(in DalamudServices, $"{Translator.GetLine("PetRenameNode.Species")}:", ActivePet?.BaseSingular ?? Translator.GetLine("..."))
                        {
                            Stylesheet = stylesheet,
                            ClassList = ["MarginSheet"],
                            Interactable = true,
                        },
                        IDNode = new RenameTitleNode(in DalamudServices, "ID:", ActivePet?.Model.ToString() ?? Translator.GetLine("..."))
                        {
                            Stylesheet = stylesheet,
                            ClassList = ["MarginSheet"],
                            Interactable = true,
                        },
                        RaceNode = new RenameTitleNode(in DalamudServices, $"{Translator.GetLine("PetRenameNode.Race")}:", ActivePet?.RaceName ?? Translator.GetLine("..."))
                        {
                            Stylesheet = stylesheet,
                            ClassList = ["MarginSheet"],
                            Interactable = true,
                        },
                        BehaviourNode = new RenameTitleNode(in DalamudServices, $"{Translator.GetLine("PetRenameNode.Behaviour")}:", ActivePet?.BehaviourName ?? Translator.GetLine("..."))
                        {
                            Stylesheet = stylesheet,
                            ClassList = ["MarginSheet"],
                            Interactable = true,
                        },
                        NicknameNode = new NicknameEditNode(in DalamudServices, $"{Translator.GetLine("PetRenameNode.Nickname")}:", CurrentValue)
                        {
                            Stylesheet = stylesheet,
                            ClassList = ["MarginSheet"],
                            Interactable = true,
                        },
                    ]
                },
                // This is the node that holds the image together
                IconNode = new IconNode()
                {
                    Style = new Style()
                    {
                        Size = new Size(90, 90),
                        Margin = new EdgeSize(20, 30),
                        IconId = ActivePet?.Icon ?? 66310,
                        BorderColor = new BorderColor(new Color(255, 255, 255)),
                        BorderWidth = new EdgeSize(4),
                        BorderRadius = 8,
                    },
                    ChildNodes =
                    [
                        CircleImageNode = new TechnoCircleImageNode(in services)
                        { 
                            Opacity = 0.3f,
                            RoationSpeed = 12,
                            Style = new Style()
                            {
                                Anchor = Anchor.MiddleCenter,
                                Size = new Size(130, 130),
                            }
                        }
                    ]
                },
            ];

        NicknameNode.OnSave += (value) => DalamudServices.Framework.Run(() => OnSave?.Invoke(value));
    }

    public void Setup(string? customName, in IPetSheetData? activePet)
    {
        ActivePet = activePet;
        CurrentValue = customName;
        IconNode.IconID = activePet?.Icon ?? 66310;
        SpeciesNode.SetText(activePet?.BaseSingular ?? Translator.GetLine("..."));
        RaceNode.SetText(activePet?.RaceName ?? Translator.GetLine("..."));
        BehaviourNode.SetText(activePet?.BehaviourName ?? Translator.GetLine("..."));
        IDNode.SetText(ActivePet?.Model.ToString() ?? Translator.GetLine("..."));
        NicknameNode.SetPet(customName, activePet);

        CircleImageNode.Style.IsVisible = activePet != null;
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        Rect activeRect = SpeciesNode.UnderlineNode.Bounds.ContentRect;
        Rect iconRect = IconNode.Bounds.ContentRect;

        Vector2 activePos = activeRect.TopRight + (activeRect.BottomRight - activeRect.TopRight) * 0.5f - new Vector2(ScaleFactor, 0);
        Vector2 iconPos = iconRect.TopLeft + (iconRect.BottomLeft - iconRect.TopLeft) * 0.5f;
        Vector2 earlyiconPos = iconPos - new Vector2(14f, 0) * ScaleFactor;

        drawList.AddLine(activePos, earlyiconPos, new Color(255, 255, 255, 255).ToUInt(), 2 * ScaleFactor);
        drawList.AddLine(earlyiconPos, iconPos, new Color(255, 255, 255, 255).ToUInt(), 2 * ScaleFactor);
    }

    readonly Stylesheet stylesheet = new Stylesheet([
        new(".MarginSheet", new Style()
        {
            Margin = new EdgeSize(2, 0, 0, 15),
        }),
    ]);
}
