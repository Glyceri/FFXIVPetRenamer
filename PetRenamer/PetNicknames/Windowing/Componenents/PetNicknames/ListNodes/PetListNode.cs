using ImGuiNET;
using PetNicknames.PetNicknames.Windowing.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Images;
using System;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.ListNodes;

internal class PetListNode : Node
{
    readonly IconNode IconNode;
    readonly QuickClearButton ClearButtonNode;

    readonly Node HolderNode;

    readonly RenameTitleNode SpeciesNode;
    readonly RenameTitleNode IDNode;
    readonly NicknameEditNode NicknameNode;

    public Action<string?>? OnSave;

    readonly DalamudServices DalamudServices;
    readonly Configuration Configuration;
    readonly IPetMode PetMode;

    public PetListNode(in IPetMode petMode, in DalamudServices services, in Configuration configuration, in IPetSheetData data, string? customName, bool asLocalEntry)
    {
        PetMode = petMode;
        Configuration = configuration;
        DalamudServices = services;

        Style = new Style()
        {
            Flow = Flow.Horizontal,
            BackgroundColor = new Color("ListElementBackground"),
            Size = new Size(412, 70),
            BorderColor = new(new("Outline")),
            BorderWidth = new EdgeSize(1),
        };

        ChildNodes = [
            HolderNode = new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    Margin = new EdgeSize(6, 0, 0, 8),
                },
                ChildNodes = [
                    SpeciesNode = new RenameTitleNode(in DalamudServices, $"{Translator.GetLine(PetMode.CurrentMode == Enums.PetWindowMode.Minion ? "PetRenameNode.Species" : "PetRenameNode.Species2")}:", data.BaseSingular),
                    IDNode = new RenameTitleNode(in DalamudServices, $"ID:", data.Model.ToString()),
                    NicknameNode = new NicknameEditNode(in DalamudServices, $"{Translator.GetLine("PetRenameNode.Nickname")}:", customName ?? Translator.GetLine("...")),
                ]
            },
            IconNode = new IconNode()
            {
                Style = new Style()
                {
                    Size = new Size(60, 60),
                    Anchor = Anchor.MiddleRight,
                    Margin = new EdgeSize(0, 26, 0, 0),
                    BorderColor = new BorderColor(new Color("Outline")),
                    BorderWidth = new EdgeSize(4),
                }
            },
            ClearButtonNode = new QuickClearButton()
            {
                Style = new Style()
                {
                    Anchor = Anchor.TopRight,
                    Margin = new EdgeSize(5),
                }
            },
        ];

        NicknameNode.SetPet(customName, data);
        NicknameNode.OnSave += (value) => OnSave?.Invoke(value);
        ClearButtonNode.OnClick += () => OnSave?.Invoke(null);

        IconNode.IconID = data.Icon;

        if (!asLocalEntry)
        {
            RemoveChild(ClearButtonNode);
            NicknameNode.UnderlineNode.RemoveChild(NicknameNode.EditButton, true);
            NicknameNode.UnderlineNode.RemoveChild(NicknameNode.ClearButton, true);
        }
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        if (!Configuration.uiFlare) return;

        Rect activeRect = SpeciesNode.UnderlineNode.Bounds.ContentRect;
        Rect iconRect = IconNode.Bounds.ContentRect;

        Vector2 activePos = activeRect.TopRight + (activeRect.BottomRight - activeRect.TopRight) * 0.5f - new Vector2(ScaleFactor, 0);
        Vector2 iconPos = iconRect.TopLeft + (iconRect.BottomLeft - iconRect.TopLeft) * 0.5f;
        Vector2 earlyiconPos = iconPos - new Vector2(12, 0) * ScaleFactor;

        drawList.AddLine(activePos, earlyiconPos + new Vector2(ScaleFactor * 0.5f, 0), new Color("Outline").ToUInt(), 2 * ScaleFactor);
        drawList.AddLine(earlyiconPos, iconPos, new Color("Outline").ToUInt(), 2 * ScaleFactor);
    }
}
