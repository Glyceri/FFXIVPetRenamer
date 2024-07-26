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
    readonly Configuration Configuration;

    readonly IconNode IconNode;

    IPetSheetData? ActivePet;
    string? CurrentValue = null;

    public Action<string?>? OnSave;

    readonly RenameTitleNode SpeciesNode;
    readonly RenameTitleNode RaceNode;
    readonly RenameTitleNode BehaviourNode;
    readonly NicknameEditNode NicknameNode;
    readonly RenameTitleNode IDNode;

    readonly Node HolderNode;

    public PetRenameNode(string? customName, in IPetSheetData? activePet, in DalamudServices services, in Configuration configuration)
    {
        DalamudServices = services;
        Configuration = configuration;

        CurrentValue = customName;
        ActivePet = activePet;
        ChildNodes =
            [
                // Rename Node part
                HolderNode = new Node()
                {
                    Stylesheet = stylesheet,
                    Style = new Style()
                    {
                        Flow = Flow.Vertical,
                        Margin = new EdgeSize(10, 0, 0, 0),
                    },

                    ChildNodes =
                    [
                        SpeciesNode = new RenameTitleNode(in DalamudServices, $"{Translator.GetLine("PetRenameNode.Species")}:", ActivePet?.BaseSingular ?? Translator.GetLine("...")) { ClassList = ["MarginSheet"] },
                        IDNode = new RenameTitleNode(in DalamudServices, "ID:", ActivePet?.Model.ToString() ?? Translator.GetLine("...")) { ClassList = ["MarginSheet"] },
                        RaceNode = new RenameTitleNode(in DalamudServices, $"{Translator.GetLine("PetRenameNode.Race")}:", ActivePet?.RaceName ?? Translator.GetLine("...")) { ClassList = ["MarginSheet"] },
                        BehaviourNode = new RenameTitleNode(in DalamudServices, $"{Translator.GetLine("PetRenameNode.Behaviour")}:", ActivePet?.BehaviourName ?? Translator.GetLine("...")) { ClassList = ["MarginSheet"] },
                        NicknameNode = new NicknameEditNode(in DalamudServices, $"{Translator.GetLine("PetRenameNode.Nickname")}:", CurrentValue) { ClassList = ["MarginSheet"] },
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
                        BorderColor = new BorderColor(new Color("Outline")),
                        BorderWidth = new EdgeSize(4),
                    },
                },
            ];

        NicknameNode.OnSave += (value) => DalamudServices.Framework.Run(() => OnSave?.Invoke(value));
    }

    public void Setup(string? customName, in IPetSheetData? activePet)
    {
        ActivePet = activePet;

        if (ActivePet != null)
        {
            SpeciesNode.SetLabel($"{Translator.GetLine("PetRenameNode.Species")}:");
            SpeciesNode.SetText(activePet?.BaseSingular ?? Translator.GetLine("..."));
            AppendNodes();
        }
        else
        {
            SpeciesNode.SetLabel($"{Translator.GetLine("PetRenameNode.PleaseSummonWarningLabel")}:");
            SpeciesNode.SetText(Translator.GetLine("PetRenameNode.PleaseSummonWarning"));
            RemoveNodes();
        }

        CurrentValue = customName;
        IconNode.IconID = activePet?.Icon ?? 66310;
        
        RaceNode.SetText(activePet?.RaceName ?? Translator.GetLine("..."));
        BehaviourNode.SetText(activePet?.BehaviourName ?? Translator.GetLine("..."));
        IDNode.SetText(ActivePet?.Model.ToString() ?? Translator.GetLine("..."));
        NicknameNode.SetPet(customName, activePet);
    }

    void RemoveNodes()
    {
        HolderNode.RemoveChild(IDNode);
        HolderNode.RemoveChild(RaceNode);
        HolderNode.RemoveChild(BehaviourNode);
        HolderNode.RemoveChild(NicknameNode);
    }

    void AppendNodes()
    {
        HolderNode.AppendChild(IDNode);
        HolderNode.AppendChild(RaceNode);
        HolderNode.AppendChild(BehaviourNode);
        HolderNode.AppendChild(NicknameNode);
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        if (!Configuration.uiFlare) return;

        Rect activeRect = SpeciesNode.UnderlineNode.Bounds.ContentRect;
        Rect iconRect = IconNode.Bounds.ContentRect;

        Vector2 activePos = activeRect.TopRight + (activeRect.BottomRight - activeRect.TopRight) * 0.5f - new Vector2(ScaleFactor, 0);
        Vector2 iconPos = iconRect.TopLeft + (iconRect.BottomLeft - iconRect.TopLeft) * 0.5f;
        Vector2 earlyiconPos = iconPos - new Vector2(14f, 0) * ScaleFactor;

        drawList.AddLine(activePos, earlyiconPos, new Color("Outline").ToUInt(), 2 * ScaleFactor);
        drawList.AddLine(earlyiconPos, iconPos, new Color("Outline").ToUInt(), 2 * ScaleFactor);
    }

    readonly Stylesheet stylesheet = new Stylesheet([
        new(".MarginSheet", new Style()
        {
            Margin = new EdgeSize(2, 0, 0, 15),
        }),
    ]);
}
