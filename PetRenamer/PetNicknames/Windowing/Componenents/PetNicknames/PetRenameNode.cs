﻿using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using System;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class PetRenameNode : Node
{
    readonly Node RenameNode;
    readonly Node ImageNode;
    readonly IconNode IconNode;
    readonly CircleImageNode CircleImageNode;

    readonly Node HeaderNode;

    readonly Node TextNode;
    readonly StringInputNode InputField;

    readonly Node SaveButton;
    readonly Node ClearButton;

    readonly IPetSheetData ActivePet;

    const int Margin = 3;

    string? CurrentValue = null;

    public Action<string?>? OnSave;

    readonly DalamudServices DalamudServices;

    public PetRenameNode(string? customName, in IPetSheetData activePet, in DalamudServices services)
    {
        DalamudServices = services;
        CurrentValue = customName;

        ActivePet = activePet;

        // HA HA
        ChildNodes = [
            RenameNode = new Node()
            {
                Stylesheet = PetRenameStyleSheet,
                ClassList = ["RenameNode"],
                ChildNodes = [
                HeaderNode = new Node()
                {
                    Stylesheet = PetRenameStyleSheet,
                    ClassList = ["RenameElementTopText", "RenameElementStyle", "RenameElementMargin"],
                    NodeValue = string.Format(Translator.GetLine(customName == null ? "PetRenameNode.IsNotRenamed" : "PetRenameNode.IsRenamed"), activePet.BaseSingular),
                },
                    TextNode = new Node()
                    {
                        Stylesheet = PetRenameStyleSheet,
                        ClassList = ["RenameElementTopText", "RenameElementStyle", "RenameElementMargin"],
                        NodeValue = customName ?? "",
                    },
                    InputField = new StringInputNode("RenameNode", customName ?? "", PluginConstants.ffxivNameSize)
                    {
                        Stylesheet = PetRenameStyleSheet,
                        ClassList = ["RenameElementStyle", "RenameElementMargin"],
                    },
                    new Node()
                    {

                        ChildNodes =
                      [
                          SaveButton = new Node()
                          {
                              Stylesheet = PetRenameStyleSheet,
                              ClassList = ["RenameElementButton"],
                              NodeValue = Translator.GetLine("PetRenameNode.SaveNickname"),
                          },
                          ClearButton = new Node()
                          {
                              Stylesheet = PetRenameStyleSheet,
                              ClassList = ["RenameElementButton"],
                              NodeValue = Translator.GetLine("PetRenameNode.ClearNickname"),
                          }
                      ]
                    }
                ]
            },
        ImageNode = new Node()
            {
                Stylesheet = PetRenameStyleSheet,
                ClassList = ["ImageNode"],
                ChildNodes = [
                    IconNode = new IconNode(in DalamudServices, activePet.Icon),
                    CircleImageNode = new CircleImageNode(in DalamudServices)
                    {
                       Style = new Style()
                        {
                           Anchor = Anchor.MiddleLeft
                        }
                    }
                    ],
            },
            
        ];

        InputField.OnValueChanged += (str) => CurrentValue = str;
        SaveButton.OnMouseUp += _ => OnSave?.Invoke(InputField.Value == string.Empty ? null : InputField.Value);
        ClearButton.OnMouseUp += _ => OnSave?.Invoke(null);

        BeforeReflow += _Reflow;
    }

    public void Setup(string? customName, in IPetSheetData activePet)
    {
        HeaderNode.NodeValue = string.Format(Translator.GetLine(customName == null ? "PetRenameNode.IsNotRenamed" : "PetRenameNode.IsRenamed"), activePet.BaseSingular);
        TextNode.NodeValue = customName ?? "";
        InputField.Value = customName ?? "";
        IconNode.IconID = activePet.Icon;
    }

    bool _Reflow(Node? node = null)
    {
        Style.Size = ParentNode!.Style!.Size;
        int height = Style!.Size!.Height;
        RenameNode.Style.Size = (Style.Size - RenameNode.ComputedStyle.Margin.Size - RenameNode.ComputedStyle.Padding.Size) - new Size(height, 0);
        ImageNode.Style.Size = new Size(height, height);
        IconNode.Style.Size = new Size(height, height);
        CircleImageNode.Style.Size = new Size(height, height);

        HeaderNode.Style.Size = new Size(RenameNode.Style.Size.Width - Margin * 2, 30);
        TextNode.Style.Size = new Size(RenameNode.Style.Size.Width - Margin * 2, 30);
        InputField.Style.Size = new Size(RenameNode.Style.Size.Width - Margin * 2, 33);

        SaveButton.Style.Size = new Size((RenameNode.Style.Size.Width - Margin * 4) / 2, 30);
        ClearButton.Style.Size = new Size((RenameNode.Style.Size.Width - Margin * 4) / 2, 30);
        return true;
    }

    static Stylesheet PetRenameStyleSheet { get; } = new Stylesheet(
        [
            new(".RenameNode", new Style()
            {
                BorderRadius = 6,
                IsAntialiased = false,
                RoundedCorners = RoundedCorners.BottomLeft,
                Padding = new EdgeSize(4, 0, 0, 0),
                StrokeRadius = 6,
                Flow = Flow.Vertical,
            }),
            new(".ImageNode", new Style()
            {
                BorderColor = new(new("Window.TitlebarBorder")),
                BorderWidth = new EdgeSize(0, 0, 0, 2),
                Padding = new EdgeSize(0),
                BorderRadius = 6,
                StrokeRadius = 6,
                IsAntialiased = false,
                RoundedCorners = RoundedCorners.BottomRight,
            }),
            new(".RenameElementStyle", new Style()
            {
                BackgroundColor = new Color("Window.BackgroundLight"),
                BorderColor = new(new("Window.TitlebarBorder")),
                BorderWidth = new EdgeSize(2, 2, 2, 0),
                BorderRadius = 8,
                StrokeRadius = 8,
                IsAntialiased = false,
                RoundedCorners = RoundedCorners.BottomRight | RoundedCorners.TopRight,
            }),
            new(".RenamePortion", new Style()
            {
                Flow = Flow.Vertical,
            }),
            new(".RenameElementMargin", new Style()
            {
                Margin = new EdgeSize(Margin, Margin, Margin, 0),
            }),
            new(".RenameElementTopText", new Style()
            {
                IsAntialiased = false,
                TextShadowSize = 2,
                Color = new Color("Window.TextLight"),
                TextShadowColor = new Color("Window.TextOutline"),
                FontSize = 15,
                TextOffset = new Vector2(0, 4f),
                Padding = new EdgeSize(Margin),
            }),
            new(".RenameElementButton", new Style()
            {
                BackgroundColor = new Color("PetNicknamesButton"),
                TextShadowSize = 2,
                TextShadowColor = new Color("Window.TextOutline"),
                FontSize = 10,
                TextAlign = Anchor.MiddleCenter,
                TextOffset = new Vector2(0, 2),
                Margin = new EdgeSize(3, Margin, 0, Margin),
                Padding = new EdgeSize(Margin, Margin, Margin, Margin),
                Color = new Color("Window.TextLight"),
                BorderColor = new(new("Window.TitlebarBorder")),
                BorderWidth = new EdgeSize(2, 2, 0, 2),
                BorderRadius = 8,
                StrokeRadius = 8,
                IsAntialiased = false,
                RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.TopRight,
            }),
            new(".RenameElementButton:hover", new Style()
            {
                BackgroundColor = new Color("PetNicknamesButton:Hover"),
                FontSize = 12,
                TextAlign = Anchor.MiddleCenter,
            }),
        ]
    );
}
