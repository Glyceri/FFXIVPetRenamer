using Dalamud.Interface;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Base.Style;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;
using PetRenamer.PetNicknames.Windowing.Enums;
using PetRenamer.PetNicknames.Windowing.Windows.PetListWindow.Enum;
using PetRenamer.PetNicknames.Windowing.Windows.PetListWindow.Structs;
using System;
using System.Collections.Generic;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

internal partial class PetListWindow : PetWindow
{
    protected override string Title { get; } = "Pet List";
    protected override string ID { get; } = "Pet List Window";

    protected override Vector2 MinSize { get; } = new Vector2(550, 240);
    protected override Vector2 MaxSize { get; } = new Vector2(550, 997);
    protected override Vector2 DefaultSize { get; } = new Vector2(550, 240);
    protected override bool HasModeToggle { get; } = true;

    readonly IPettableUserList UserList;
    readonly IPettableDatabase Database;
    readonly IPettableDatabase LegacyDatabase;
    readonly IPetServices PetServices;
    readonly IImageDatabase ImageDatabase;

    bool inUserMode = false;
    bool lastInUserMode = false;
    IPettableDatabaseEntry? ActiveEntry;
    IPettableUser? lastUser = null;

    readonly UserNode UserNode;

    readonly Node HeaderNode;
    readonly Node ScrollListBaseNode;
    readonly Node ScrollistContentNode;
    readonly Node BottomPortion;

    readonly Node NextListNode;
    readonly Node PreviousListNode;

    readonly QuickButton UserListButton;
    readonly QuickButton SharingButton;
    readonly QuickButton ActiveEveryoneButton;

    readonly SmallHeaderNode SmallHeaderNode;

    bool isLocalEntry = false;

    public const int ElementsPerList = 10;
    int currentIndex = 0;


    public PetListWindow(in DalamudServices dalamudServices, in IPetServices petServices, in IPettableUserList userList, in IPettableDatabase database, IPettableDatabase legacyDatabase, in IImageDatabase imageDatabase) : base(dalamudServices, "Pet List")
    {
        UserList = userList;
        Database = database;
        LegacyDatabase = legacyDatabase;
        PetServices = petServices;
        ImageDatabase = imageDatabase;
        ContentNode.Style.Flow = Flow.Vertical;
        ContentNode.Stylesheet = stylesheet;

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
                            BorderColor = new(new("Window.TitlebarBorder")),
                            BorderWidth = new EdgeSize(0, 1, 1, 1),
                            RoundedCorners = RoundedCorners.BottomLeft,
                            BorderRadius = 6,
                            StrokeRadius = 6,
                            IsAntialiased = false,
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
                            BorderColor = new(new("Window.TitlebarBorder")),
                            BorderWidth = new EdgeSize(0, 1, 1, 1),
                            RoundedCorners = RoundedCorners.BottomRight,
                            BorderRadius = 6,
                            StrokeRadius = 6,
                            IsAntialiased = false,
                            Flow = Flow.Vertical,
                        },
                        ChildNodes = [
                    new SmallHeaderNode("Navigation")
                    {
                        Style = new Style()
                        {
                            Margin = new EdgeSize(5),
                            Size = new Size(60, 20),
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
                                },
                                ChildNodes =
                                [
                                    UserListButton = new QuickButton(in DalamudServices, "User List")
                                    {
                                        Style = new Style()
                                        {
                                            Size = new Size(60, 15),
                                            Margin = new EdgeSize(18, 1, 1, 1),
                                            Anchor = Anchor.TopCenter,
                                        },
                                    },
                                    SharingButton = new QuickButton(in DalamudServices, "Sharing")
                                    {
                                        Style = new Style()
                                        {
                                            Size = new Size(60, 15),
                                            Margin = new EdgeSize(1),
                                            Anchor = Anchor.TopCenter,
                                        },
                                    },
                                    ActiveEveryoneButton = new QuickButton(in DalamudServices, "Active Everyone")
                                    {
                                        Style = new Style()
                                        {
                                            Size = new Size(60, 15),
                                            Margin = new EdgeSize(18, 1, 1, 1),
                                            Anchor = Anchor.TopCenter,
                                        },
                                    },
                                ]
                            }
                        ],
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
                new Node()
                {
                    Style = new Style()
                    {
                        Flow = Flow.Vertical,
                        Anchor = Anchor.TopRight,
                        Gap = 5,
                        Margin = new EdgeSize(32, 37, 0, 0),
                    },
                    ChildNodes =
                        [
                            NextListNode = new Node()
                            {
                                ClassList = ["ListButton"],
                                NodeValue = FontAwesomeIcon.ArrowRight.ToIconString(),
                            },
                            PreviousListNode = new Node()
                            {
                                ClassList = ["ListButton"],
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
                            ScrollbarThumbColor = new Color(224, 183, 18, 50),
                            ScrollbarThumbHoverColor = new Color(224, 183, 18, 200),
                            ScrollbarThumbActiveColor = new Color(237, 197, 33, 255),
                            //ScrollbarRounding = 15,
                            //ScrollbarWidth = 11,
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
                    BackgroundGradient = GradientColor.Vertical(WindowStyles.WindowBorderActive, new Color(224, 183, 18, 0)),
                    RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.TopRight,
                    BorderRadius = 6,
                    Margin = new(129, 0, 0, 0),
                    Size = new Size(422, 4),
                    Anchor = Anchor.TopCenter,
                }
            },
            new Node()
            {
                Style = new Style()
                {
                    BackgroundGradient = GradientColor.Vertical(new Color(224, 183, 18, 0), WindowStyles.WindowBorderActive),
                    RoundedCorners = RoundedCorners.BottomRight | RoundedCorners.BottomLeft,
                    BorderRadius = 6,
                    Margin = new(0, 0, 29, 0),
                    Size = new Size(422, 4),
                    Anchor = Anchor.BottomCenter,
                }
            },
        ];

        UserListButton.Clicked += ToggleUserMode;

        NextListNode.OnMouseUp += _ => DalamudServices.Framework.Run(() => HandleIncrement(1));
        PreviousListNode.OnMouseUp += _ => DalamudServices.Framework.Run(() => HandleIncrement(-1));
    }

    public override void OnDraw()
    {
        BottomPortion.Style.Size = new Size(ContentSize.Width, ContentSize.Height - 100);
        ScrollListBaseNode.Style.Size = new Size(BottomPortion.Style.Size.Width - 120, BottomPortion.Style.Size.Height - 60);

        if (lastUser != UserList.LocalPlayer)
        {
            lastUser = UserList.LocalPlayer;
            SetUser(lastUser?.DataBaseEntry);
        }

        bool dirty = false;
        foreach (IPettableDatabaseEntry e in Database.DatabaseEntries)
        {
            if (!e.IsActive) continue;
            if (e.IsDirtyForUI)
            {
                dirty = true;
                break;
            }
        }
        if (dirty)
        {
            SetUser(ActiveEntry);
        }
    }

    void HandleIncrement(int amount)
    {
        SetIndex(currentIndex + amount);
    }

    void SetIndex(int amount)
    {
        currentIndex = amount;
        PreviousListNode.IsDisabled = currentIndex == 0;
        SetUser(ActiveEntry);
    }

    protected override void OnPetModeChanged(PetWindowMode mode)
    {
        if (inUserMode) return;
        SetIndex(0);
    }

    void ToggleUserMode()
    {
        inUserMode = !inUserMode;
        SetUser(ActiveEntry);
    }

    public void SetUser(IPettableDatabaseEntry? entry)
    {
        isLocalEntry = HandleIfLocalEntry(entry);

        bool completeUserChange = ActiveEntry != entry;
        ActiveEntry = entry;

        if (lastInUserMode != inUserMode || completeUserChange)
        {
            lastInUserMode = inUserMode;
            SetIndex(0);
            return;
        }

        UserNode.SetUser(entry);
        ScrollistContentNode.ChildNodes.Clear();

        SmallHeaderNode.NodeValue = "...";

        if (inUserMode) HandleUserMode();
        else HandlePetMode();
    }

    bool HandleIfLocalEntry(IPettableDatabaseEntry? entry)
    {
        if (UserList.LocalPlayer != null && entry != null)
        {
            return UserList.LocalPlayer.ContentID == entry.ContentID;
        }
        else
        {
            return false;
        }
    }

    void HandleUserMode()
    {
        IPettableDatabaseEntry[] entries = Database.DatabaseEntries;
        int length = entries.Length;

        Looper(length, (index) =>
        {
            IPettableDatabaseEntry entry = entries[index];
            if (!entry.IsActive) return false;

            bool isLocal = HandleIfLocalEntry(entry);

            UserListNode userNode = new UserListNode(in DalamudServices, in ImageDatabase, in entry, isLocal);
            ScrollistContentNode.AppendChild(userNode);

            userNode.OnView += (user) =>
            {
                inUserMode = false;
                SetUser(user);
            };

            return true;
        });
    }

    void HandlePetMode()
    {
        if (ActiveEntry == null) return;
        INamesDatabase names = ActiveEntry.ActiveDatabase;
        List<int> validIDS = new List<int>();
        List<string> validNames = new List<string>();

        int length = names.IDs.Length;

        for (int i = 0; i < length; i++)
        {
            int id = names.IDs[i];
            if (PetWindowMode.Minion    == CurrentMode && id <= -1) continue;
            if (PetWindowMode.BattlePet == CurrentMode && id >= -1) continue;

            string cusomName = names.Names[i];

            validIDS.Add(id);
            validNames.Add(cusomName);
        }

        int newLength = validIDS.Count;

        Looper(newLength, (index) =>
        {
            int id = validIDS[index];
            if (CurrentMode == PetWindowMode.Minion && id <= -1) return false;
            if (CurrentMode == PetWindowMode.BattlePet && id >= -1) return false;

            IPetSheetData? petData = PetServices.PetSheets.GetPet(id);
            if (petData == null) return false;

            string customName = validNames[index];

            PetListNode newPetListNode = new PetListNode(in DalamudServices, petData, customName, isLocalEntry);
            ScrollistContentNode.AppendChild(newPetListNode);
            newPetListNode.OnSave += (value) => OnSave(value, id);

            return true;
        });
    }

    void Looper(int length, Func<int, bool> onValidCallback)
    {
        OffsetHelper offsetHelper = new OffsetHelper(currentIndex);

        NextListNode.IsDisabled = true;

        for (int i = 0; i < length; i++)
        {
            OffsetResult result = offsetHelper.OffsetResult();
            if (result == OffsetResult.Early) continue;
            if (result == OffsetResult.Late)
            {
                NextListNode.IsDisabled = false;
                break;
            }

            if (!onValidCallback.Invoke(i)) continue;

            offsetHelper.IncrementValidOffset();
        }
    }

    void OnSave(string? newName, int skeleton) => DalamudServices.Framework.Run(() => ActiveEntry?.SetName(skeleton, newName ?? ""));
}
