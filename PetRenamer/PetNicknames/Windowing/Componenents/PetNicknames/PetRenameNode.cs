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
    string inputFieldvalue = "";


    public Action<string?>? OnSave;

    readonly CircleImageNode CircleImageNode;

    readonly RenameTitleNode SpeciesNode;
    readonly RenameTitleNode RaceNode;
    readonly RenameTitleNode BehaviourNode;
    readonly RenameTitleNode NicknameNode;
    readonly RenameTitleNode IDNode;

    Node? activeHoverNode;

    readonly QuickButton EditButton;
    readonly QuickButton ClearButton;

    bool editMode = false;

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
                    NicknameNode = new RenameTitleNode($"{Translator.GetLine("PetRenameNode.Nickname")}:", CurrentValue ?? "...")
                    {
                        Stylesheet = stylesheet,
                        ClassList = ["MarginSheet"],
                    },
                    new Node()
                    {
                        Style = new Style()
                        {
                            Flow = Flow.Horizontal,
                            Margin = new EdgeSize(0, 0, 0, 200),
                        },
                        ChildNodes = [
                        EditButton = new QuickButton($"{Translator.GetLine("PetRenameNode.Edit")}:"),
                            ClearButton = new QuickButton($"{Translator.GetLine("PetRenameNode.Clear")}:"),
                        ]
                    },
                ]
            },
            // This is the node that holds the image together
            new Node()
            {
                //Overflow = false,
                Style = new Style()
                {
                    Margin = new EdgeSize(19, 10, 0, 0),
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

        ClearButton.Clicked += ClearClicked;
        EditButton.Clicked += EditClicked;
    }

    void ClearClicked()
    {
        if (editMode) StopEditMode();
        else OnSave?.Invoke(null);
    }

    void EditClicked()
    {
        if (editMode) 
        { 
            OnSave?.Invoke(inputFieldvalue); 
            StopEditMode(); 
        }
        else StartEditMode();
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

        ClearButton.Style.IsVisible = activePet != null;
        EditButton.Style.IsVisible = activePet != null;
        CircleImageNode.Style.IsVisible = activePet != null;

        StopEditMode();
    }

    void StartEditMode()
    {
        EditButton.SetText($"{Translator.GetLine("PetRenameNode.Save")}");
        ClearButton.SetText($"{Translator.GetLine("PetRenameNode.Cancel")}");
        NicknameNode.SetText("");
        editMode = true;
    }

    void StopEditMode()
    {
        editMode = false;
        ClearButton.SetText($"{Translator.GetLine("PetRenameNode.Clear")}");
        EditButton.SetText($"{Translator.GetLine("PetRenameNode.Edit")}");
        NicknameNode.SetText(CurrentValue ?? "...");
        inputFieldvalue = CurrentValue ?? string.Empty;
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        base.OnDraw(drawList);


        if (editMode)
        {
            ImGui.SetCursorPos(new Vector2(89 * Node.ScaleFactor, 115 * Node.ScaleFactor));
            ImGui.SetNextItemWidth(NicknameNode.TextNode.Bounds.ContentRect.Width - NicknameNode.LabelNode.Bounds.ContentRect.Width);
            if (ImGui.InputText($"##RenameField", ref inputFieldvalue, PluginConstants.ffxivNameSize, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.None))
            {
                EditButton.Clicked?.Invoke();
            }
        }

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
        Vector2 earlyiconPos = iconPos - new Vector2(45, 0);

        drawList.AddLine(activePos, earlyiconPos, new Color(255, 255, 255, 255).ToUInt(), 2 * Node.ScaleFactor);
        drawList.AddLine(earlyiconPos, iconPos, new Color(255, 255, 255, 255).ToUInt(), 2 * Node.ScaleFactor);
    }

    Stylesheet stylesheet = new Stylesheet([
        new(".MarginSheet", new Style()
        {
            Margin = new EdgeSize(4, 0, 0, 15),
        }),
    ]);
}
