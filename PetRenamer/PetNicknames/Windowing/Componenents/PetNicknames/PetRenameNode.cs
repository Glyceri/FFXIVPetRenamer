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
    readonly Node RenameNode;
    readonly Node ImageNode;
    readonly IconNode IconNode;

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
                              ClassList = ["RenameElementStyle", "RenameElementMargin", "RenameElementButton"],
                              NodeValue = Translator.GetLine("PetRenameNode.SaveNickname"),
                          },
                          ClearButton = new Node()
                          {
                              Stylesheet = PetRenameStyleSheet,
                              ClassList = ["RenameElementStyle", "RenameElementMargin", "RenameElementButton"],
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
                  ChildNodes =
                      [
                          IconNode = new IconNode(in DalamudServices, activePet.Icon)
                          {
                              ClassList = ["IconElement"],
                          }
                      ],
                  }
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
                StrokeRadius = 6,
                BorderColor = new(new("Window.TitlebarBorder")),
                BorderWidth = new EdgeSize(1),
                Flow = Flow.Vertical,
            }),
            new(".ImageNode", new Style()
            {
                BorderColor = new(new("Window.TitlebarBorder")),
                BorderWidth = new EdgeSize(0, 0, 0, 1),
                BorderRadius = 6,
                StrokeRadius = 6,
                IsAntialiased = false,
                RoundedCorners = RoundedCorners.BottomRight,
                ShadowSize = new EdgeSize(0, 0, 0, 32),
            }),
            new(".RenameElementStyle", new Style()
            {
                BackgroundColor = new Color("Window.Background"),
                BorderColor = new(new("Window.TitlebarBorder")),
                BorderWidth = new EdgeSize(1, 1, 1, 1),
                BorderRadius = 6,
                StrokeRadius = 6,
                IsAntialiased = false,
                RoundedCorners = RoundedCorners.All,
            }),
            new(".RenamePortion", new Style()
            {
                Flow = Flow.Vertical,
            }),
            new(".RenameElementMargin", new Style()
            {
                Margin = new EdgeSize(Margin),
            }),
            new(".RenameElementTopText", new Style()
            {
                TextShadowSize = 2,
                TextShadowColor = new Color("Window.TitlebarTextOutline"),
                FontSize = 15,
                TextOffset = new Vector2(0, 4f),
                Padding = new EdgeSize(Margin),
            }),
            new(".RenameElementButton", new Style()
            {
                BackgroundColor = new Color("Window.BackgroundLight"),
                TextShadowSize = 2,
                TextShadowColor = new Color("Window.TitlebarTextOutline"),
                FontSize = 15,
                TextAlign = Anchor.MiddleCenter,
                TextOffset = new Vector2(0, 2),
                Padding = new EdgeSize(Margin),
            }),
            new(".RenameElementButton:hover", new Style()
            {
                BackgroundColor = new Color("Window.Background"),
            }),
            new(".IconElement", new Style()
            {
                Margin = new(0),                
            })
        ]
    );
}
