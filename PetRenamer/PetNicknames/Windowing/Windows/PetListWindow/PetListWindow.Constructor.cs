using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.Parsing.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.ReadingAndParsing.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.WindowNodes;
using PetRenamer.PetNicknames.WritingAndParsing.DataParseResults;
using PetRenamer.PetNicknames.WritingAndParsing.Interfaces.IParseResults;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

internal partial class PetListWindow
{
    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPettableDatabase LegacyDatabase;
    readonly IPetServices PetServices;
    readonly IImageDatabase ImageDatabase;

    readonly IDataParser DataParser;
    readonly IDataWriter DataWriter;

    readonly UserNode UserNode;

    readonly Node HeaderNode;
    readonly Node ScrollListBaseNode;
    readonly Node ScrollistContentNode;
    readonly Node BottomPortion;

    readonly QuickSquareButton SearchModeNode;
    readonly QuickSquareButton NextListNode;
    readonly QuickSquareButton PreviousListNode;

    readonly SearchBarNode SearchBarNode;

    readonly SmallHeaderNode SmallHeaderNode;

    readonly QuickSquareButton ExportButton;
    readonly QuickSquareButton ImportButton;

    readonly QuickSquareButton PetListButton;

    public PetListWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, in IPetServices petServices, in IPettableUserList userList, in IPettableDatabase database, IPettableDatabase legacyDatabase, in IImageDatabase imageDatabase, in IDataParser dataParser, in IDataWriter dataWriter) : base(windowHandler, dalamudServices, configuration, "Pet List")
    {
        IsOpen = false;

        UserList = userList;
        Database = database;
        LegacyDatabase = legacyDatabase;
        PetServices = petServices;
        ImageDatabase = imageDatabase;
        DataParser = dataParser;
        DataWriter = dataWriter;

        ContentNode.Style.Flow = Flow.Vertical;

        ContentNode.ChildNodes = [
            HeaderNode = new Node()
            {
                Style = new Style()
                {
                    Size = new Size(540, 100),
                },
                ChildNodes = [
                    new Node()
                    {
                        Style = new Style()
                        {
                            Size = new Size(410, 100),
                            BorderColor = new(new("Outline")),
                            BorderWidth = new EdgeSize(0, 1, 1, 1),
                        },
                        ChildNodes =
                        [
                            UserNode = new UserNode(in DalamudServices, in ImageDatabase),
                        ]
                    },
                    new Node()
                    {
                        Style = new Style()
                        {
                            Size = new Size(131, 100),
                            BorderColor = new(new("Outline")),
                            BorderWidth = new EdgeSize(0, 1, 1, 1),
                            Flow = Flow.Vertical,
                        },
                        ChildNodes = [
                            new SmallHeaderNode(Translator.GetLine("PetList.Sharing"))
                            {
                                Style = new Style()
                                {
                                    Margin = new EdgeSize(5),
                                    Size = new Size(90, 20),
                                    Anchor = Anchor.TopCenter,
                                    
                                }
                            },
                            new Node()
                            {
                                Style = new Style()
                                {
                                    Margin = new EdgeSize(5),
                                    Size = new Size(120, 80),
                                    Anchor = Anchor.BottomCenter,
                                    Flow = Flow.Vertical,
                                    Padding = new EdgeSize(12, 0, 15, 0),
                                },
                                ChildNodes =
                                [
                                     ExportButton = new QuickSquareButton()
                                     {
                                         Style = new Style()
                                         {
                                             Size = new Size(50, 20),
                                             Anchor = Anchor.TopCenter,
                                             Margin = new EdgeSize(12, 1, 1, 1),
                                         },
                                         NodeValue = FontAwesomeIcon.FileExport.ToIconString(),
                                         Tooltip = Translator.GetLine("ShareWindow.Export"),
                                     },
                                    ImportButton = new QuickSquareButton()
                                    {
                                        Style = new Style()
                                        {
                                            Size = new Size(50, 20),
                                            Anchor = Anchor.TopCenter,
                                            Margin = new EdgeSize(1),
                                        },
                                        NodeValue = FontAwesomeIcon.FileImport.ToIconString(),
                                        Tooltip = Translator.GetLine("ShareWindow.Import"),
                                    },
                                ],
                            }
                        ]
                    }
                ]
            },
            BottomPortion = new Node()
            {
                ChildNodes = [
                    SmallHeaderNode = new SmallHeaderNode(Translator.GetLine("PetListWindow.ListHeaderPersonalMinion"))
                    {
                        Style = new Style()
                        {
                            Anchor = Anchor.TopCenter,
                            Margin = new EdgeSize(5, 0, 0, 0),
                            Size = new Size(300, 20),
                            FontSize = 16,
                        }
                    },
                    SearchBarNode = new SearchBarNode(in DalamudServices, Translator.GetLine("Search"), "")
                    {
                        Style = new Style()
                        {
                            Anchor = Anchor.TopCenter,
                            Margin = new EdgeSize(10, 0, 0, 0),
                            ScrollbarTrackColor = new Color(0, 0, 0, 0),
                            ScrollbarThumbColor = new Color("Button.Background"),
                            ScrollbarThumbHoverColor = new Color("Button.Background:Hover"),
                            ScrollbarThumbActiveColor = new Color("Button.Background:Active"),
                        },
                    },
                    new Node()
                    {
                        Style = new Style()
                        {
                            Flow = Flow.Vertical,
                            Anchor = Anchor.TopRight,
                            Gap = 1,
                            Margin = new EdgeSize(10, 38, 0, 0),
                        },
                        ChildNodes =
                        [
                            SearchModeNode = new QuickSquareButton()
                            {
                                Style = new Style()
                                {
                                    Size = new Size(20, 20),
                                },
                                NodeValue = FontAwesomeIcon.Search.ToIconString(),
                            },
                            PetListButton = new QuickSquareButton()
                            {
                                Style = new Style()
                                {
                                    Size = new Size(20, 20),
                                },
                                NodeValue = FontAwesomeIcon.PersonRays.ToIconString(),
                            },
                            NextListNode = new QuickSquareButton()
                            {
                                Style = new Style()
                                {
                                    Size = new Size(20, 20),
                                },
                                NodeValue = FontAwesomeIcon.ArrowRight.ToIconString(),
                            },
                            PreviousListNode = new QuickSquareButton()
                            {
                                Style = new Style()
                                {
                                    Size = new Size(20, 20),
                                },
                                NodeValue = FontAwesomeIcon.ArrowLeft.ToIconString(),
                            },
                        ]
                    },
                    ScrollListBaseNode = new Node()
                    {
                        Overflow = false,
                        Style = new Style()
                        {
                            Anchor = Anchor.MiddleCenter,
                            ScrollbarTrackColor = new Color(0, 0, 0, 0),
                            ScrollbarThumbColor = new Color("Button.Background"),
                            ScrollbarThumbHoverColor = new Color("Button.Background:Hover"),
                            ScrollbarThumbActiveColor = new Color("Button.Background:Inactive"),
                        },
                        ChildNodes =
                        [
                            ScrollistContentNode = new Node()
                            {
                                Style = new Style()
                                {
                                    Flow = Flow.Vertical,
                                    Padding = new EdgeSize(3),
                                    Gap = 10,
                                }
                            }
                        ]
                    }
                ]
            },
            new Node()
            {
                Style = new Style()
                {
                    BackgroundGradient = GradientColor.Vertical(new Color("Outline"), new Color(224, 183, 18, 0)),
                    Margin = new(129, 0, 0, 0),
                    Size = new Size(422, 2),
                    Anchor = Anchor.TopCenter,
                }
            },
            new Node()
            {
                Style = new Style()
                {
                    BackgroundGradient = GradientColor.Vertical(new Color(224, 183, 18, 0), new Color("Outline")),
                    Margin = new(0, 0, 29, 0),
                    Size = new Size(422, 2),
                    Anchor = Anchor.BottomCenter,
                }
            },
        ];

        SearchModeNode.OnClick += () => DalamudServices.Framework.Run(() => ToggleSearchMode());
        NextListNode.OnClick += () => DalamudServices.Framework.Run(() => HandleIncrement(1));
        PreviousListNode.OnClick += () => DalamudServices.Framework.Run(() => HandleIncrement(-1));
        PetListButton.OnClick += () => ToggleUserMode();

        SearchBarNode.OnSave += _ => DalamudServices.Framework.Run(() => SetUser(ActiveEntry));
        SearchBarNode.Style.IsVisible = false;

        ExportButton.OnClick += () => DalamudServices.Framework.Run(() =>
        {
            string data = DataWriter.WriteData();
            if (data.IsNullOrWhitespace())
            {
                DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                {
                    Type = Dalamud.Interface.ImGuiNotification.NotificationType.Warning,
                    Content = Translator.GetLine("ShareWindow.ExportError"),
                });
            }
            else
            {
                DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                {
                    Type = Dalamud.Interface.ImGuiNotification.NotificationType.Success,
                    Content = Translator.GetLine("ShareWindow.ExportSuccess"),
                });

                ImGui.SetClipboardText(data);
            }
        });

        ImportButton.OnClick += () => DalamudServices.Framework.Run(() =>
        {
            IDataParseResult parseResult = DataParser.ParseData(ImGui.GetClipboardText());

            if (!DataParser.ApplyParseData(parseResult, false))
            {
                string error = string.Empty;
                if (parseResult is InvalidParseResult invalidParseResult) error = invalidParseResult.Reason;

                DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                {
                    Type = Dalamud.Interface.ImGuiNotification.NotificationType.Warning,
                    Content = string.Format(Translator.GetLine("ShareWindow.ImportError"), error)
                });
            }
            else
            {
                string username = string.Empty;
                if (parseResult is IBaseParseResult baseResult) username = baseResult.UserName;

                StartDisabledTimer();

                DalamudServices.NotificationManager.AddNotification(new Dalamud.Interface.ImGuiNotification.Notification()
                {
                    Type = Dalamud.Interface.ImGuiNotification.NotificationType.Success,
                    Content = string.Format(Translator.GetLine("ShareWindow.ImportSuccess"), username)
                });
            }
        });
    }

    double internalDisabledTimer = 0;

    void StartDisabledTimer()
    {
        if (ImportButton.IsDisabled) return;

        ImportButton.IsDisabled = true;
        internalDisabledTimer = 4;
    }
}
