using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;
using PetRenamer.PetNicknames.ColourProfiling;
using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using PetRenamer.PetNicknames.Parsing.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Base.Style;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.ColourEditorWindow;

internal class ColourEditorWindow : PetWindow
{
    protected override Vector2 MinSize { get; } = new Vector2(338, 300);
    protected override Vector2 MaxSize { get; } = new Vector2(338, 1200);
    protected override Vector2 DefaultSize { get; } = new Vector2(338, 600);
    protected override bool HasModeToggle { get; } = false;
    protected override bool HasExtraButtons { get; } = false;

    protected override string Title { get; } = Translator.GetLine("ColourEditorWindow.Title");
    protected override string ID { get; } = "ColourEditorWindow";

    readonly IColourProfileHandler ColourProfileHandler;

    List<ColourSettingsNode> colourSettingNodes = new List<ColourSettingsNode>();

    bool didActivate = false;

    readonly SettingsHolderNode PresetList;
    readonly SettingsHolderNode ColourHolderNode;

    readonly IDataParser DataParser;
    readonly IDataWriter DataWriter;

    public ColourEditorWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, in IColourProfileHandler colourProfileHandler, in IDataParser dataParser, in IDataWriter dataWriter) : base(windowHandler, dalamudServices, configuration, "ColourEditorWindow")
    {
        IsOpen = false;

        ColourProfileHandler = colourProfileHandler;

        DataParser = dataParser;
        DataWriter = dataWriter;

        ContentNode.Overflow = false;

        ContentNode.Style = new Style()
        {
            ScrollbarTrackColor = new Color(0, 0, 0, 0),
            ScrollbarThumbColor = new Color("Button.Background"),
            ScrollbarThumbHoverColor = new Color("Button.Background:Hover"),
            ScrollbarThumbActiveColor = new Color("Button.Background:Active"),
        };

        ContentNode.ChildNodes = 
        [
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                    ScrollbarTrackColor = new Color(0, 0, 0, 0),
                    ScrollbarThumbColor = new Color("Button.Background"),
                    ScrollbarThumbHoverColor = new Color("Button.Background:Hover"),
                    ScrollbarThumbActiveColor = new Color("Button.Background:Active"),
                },
                ChildNodes = [
                    PresetList = new SettingsHolderNode(in Configuration, Translator.GetLine("ColourSettings.PresetListHeader")),
                    ColourHolderNode = new SettingsHolderNode(in Configuration, Translator.GetLine("ColourSettings.Header"))
                    {
                        Style = new Style()
                        {
                            Flow = Flow.Vertical,
                        }
                    },
                ]
            }
        ];

        Register("Outline");
        Register("Outline:Fade");
        Register("Window.Background");
        Register("Window.BackgroundLight");
        Register("BackgroundImageColour");
        Register("SearchBarBackground");
        Register("ListElementBackground");
        Register("Window.TextLight");
        Register("Window.TextOutline");
        Register("Window.Text");
        Register("Window.TextOutlineButton");
        Register("WindowBorder:Active");
        Register("WindowBorder:Inactive");
        Register("Button.Background");
        Register("Button.Background:Hover");
        Register("Button.Background:Inactive");

        OnPresetListChanged();
    }

    void Register(string name)
    {
        ColourSettingsNode node = new ColourSettingsNode(in Configuration, name, () =>
        {
            didActivate = true;
        });
        colourSettingNodes.Add(node);
        ColourHolderNode.ContentNode.ChildNodes.Add(node);
    }


    public void OnPresetListChanged()
    {
        PresetList.ContentNode.ChildNodes.Clear();
        QuickSquareButton importButton;
        PresetList.ContentNode.ChildNodes.Add(importButton = new QuickSquareButton());
        importButton.Style.Size = new Size(32, 15);
        importButton.NodeValue = FontAwesomeIcon.FileImport.ToIconString();
        importButton.OnClick += () => DalamudServices.Framework.Run(() => 
        {
            IDataParseResult result = DataParser.ParseData(ImGui.GetClipboardText());
            if (result is not IColourParseResult colourParseResult) 
            {
                DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                {
                    Type = Dalamud.Interface.ImGuiNotification.NotificationType.Warning,
                    Content = Translator.GetLine("ColourEditorWindow.ImportError"),
                });
            }
            else
            {
                DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                {
                    Type = Dalamud.Interface.ImGuiNotification.NotificationType.Warning,
                    Content = Translator.GetLine("ColourEditorWindow.ImportSuccess"),
                });

                ColourProfileHandler.AddColourProfile(new ColourProfile(colourParseResult.ThemeName, colourParseResult.ThemeAuthor, colourParseResult.Colours.ToList()));
            }
        });

        int activeIndex = ColourProfileHandler.GetActiveAsSerialized();

        int index = -1;
        ColourProfileConfig cConfig = AddColourprofile(WindowStyles.DefaultColourProfile, index, index == activeIndex);
        index++;

        cConfig.HolderNode.RemoveChild(cConfig.ClearButton);
        cConfig.HolderNode.RemoveChild(cConfig.ExportButton);

        foreach (IColourProfile cProfile in ColourProfileHandler.ColourProfiles)
        {
            AddColourprofile(cProfile, index, index == activeIndex);
            index++;
        }

        IColourProfile activeProfile = ColourProfileHandler.GetActiveProfile();

        foreach (ColourSettingsNode node in ColourHolderNode.ContentNode.ChildNodes)
        {
            node.UpdateProfile(activeProfile);
        }

        Node node2;
        SearchBarNode nameNode;
        SearchBarNode authorNode;
        QuickSquareButton addButton;
        PresetList.ContentNode.ChildNodes.Add(node2 = new Node()
        {
            Style = new Style()
            {
                Flow = Flow.Vertical,
                BorderColor = new(new("Outline")),
                BorderWidth = new EdgeSize(1),
                Size = new Size(304, 46),
                BackgroundColor = new Color("ListElementBackground"),
            },
            ChildNodes = 
            [
                nameNode = new SearchBarNode(in DalamudServices, Translator.GetLine("ColourEditorWindow.Name"), string.Empty),
                authorNode = new SearchBarNode(in DalamudServices, Translator.GetLine("ColourEditorWindow.Author"), string.Empty),
                addButton = new QuickSquareButton()
                {
                    Style = new Style()
                    {
                        Anchor = Anchor.BottomRight,
                    },
                    NodeValue = FontAwesomeIcon.Plus.ToIconString(),
                },
            ]
        });

        addButton.OnClick += () =>
        {
            string nameNodeField = nameNode.InputFieldvalue;
            string authorField = authorNode.InputFieldvalue;

            nameNodeField = nameNodeField.Replace(PluginConstants.forbiddenCharacter.ToString(), string.Empty);
            authorField = authorField.Replace(PluginConstants.forbiddenCharacter.ToString(), string.Empty);

            if (nameNodeField.IsNullOrWhitespace()) return;
            if (authorField.IsNullOrWhitespace()) return;

            DalamudServices.Framework.Run(() =>
            {
                ColourProfileHandler.AddColourProfile(new ColourProfile(nameNodeField, authorField, WindowStyles.DefaultColourProfile.Colours.ToList()));
                Configuration.Save();
            });
        };
    }

    ColourProfileConfig AddColourprofile(IColourProfile cProfile, int index, bool active)
    {
        ColourProfileConfig cConfig = new ColourProfileConfig(in Configuration, in DalamudServices, cProfile, index, active, (value) =>
        {
            DalamudServices.Framework.Run(() =>
            {
                ColourProfileHandler.SetActiveProfile(cProfile);
                WindowHandler?.GetWindow<ColourEditorWindow>()?.OnPresetListChanged();
                Configuration.Save();
            }); 
        },
        () => 
            {
                string exportData = DataWriter.WriteColourData(cProfile);

                if (exportData.IsNullOrWhitespace())
                {
                    DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                    {
                        Type = Dalamud.Interface.ImGuiNotification.NotificationType.Warning,
                        Content = Translator.GetLine("ColourEditorWindow.ExportError"),
                    });
                }
                else
                {
                    DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                    {
                        Type = Dalamud.Interface.ImGuiNotification.NotificationType.Success,
                        Content = Translator.GetLine("ColourEditorWindow.ExportSuccess"),
                    });

                    ImGui.SetClipboardText(exportData);
                }
            }
        );

        cConfig.ClearButton.OnClick += () =>
        {
            DalamudServices.Framework.Run(() =>
            {
                ColourProfileHandler.RemoveColourProfile(cProfile);
                Configuration.Save();
            });
        };

        PresetList.ContentNode.ChildNodes.Add(cConfig);

        return cConfig;
    }

    public override void OnDraw()
    {
        int activeIndex = ColourProfileHandler.GetActiveAsSerialized();

        ColourHolderNode.Style.IsVisible = activeIndex != -1;

        if (didActivate)
        {
            didActivate = false;
            ColourProfileHandler.Refresh();
            Configuration.Save();
        }
    }
}
