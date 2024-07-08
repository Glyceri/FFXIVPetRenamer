using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Enums;
using System;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class ModeToggleNode : Node
{
    Node CompanionNode => QuerySelector("MinionMode")!;
    Node BattlePetNode => QuerySelector("BattlePetMode")!;

    public event Action<PetWindowMode>? OnModeChange;

    public ModeToggleNode(in DalamudServices dalamudServices)
    {
        Id = "BaseModeToggleNode";
        Stylesheet = ModeToggleStylesheet;
        ClassList = ["ModeToggleBase"];

        ChildNodes = [
            new()
            {
                Id = "MinionMode",
                Stylesheet = ModeToggleStylesheet,
                ClassList = ["ModeToggleUnavailableMinion"],
                Tooltip = "Switch to Minion Mode"
            },
            new()
            {
                Id = "BattlePetMode",
                Stylesheet = ModeToggleStylesheet,
                ClassList = ["BattlePetModeActive"],
                Tooltip = "Switch to Battle Pet Mode"
            },
        ];

        CompanionNode.OnClick += _ => OnModeChange?.Invoke(PetWindowMode.Minion);
        BattlePetNode.OnClick += _ => OnModeChange?.Invoke(PetWindowMode.BattlePet);
    }

    public void SetActivePetMode(PetWindowMode mode)
    {
        if (mode == PetWindowMode.Minion)
        {
            CompanionNode.ClassList = ["ModeToggleUnavailableMinion"];
            BattlePetNode.ClassList = ["BattlePetModeActive"];
        }
        else if (mode == PetWindowMode.BattlePet)
        {
            CompanionNode.ClassList = ["MinionModeActive"];
            BattlePetNode.ClassList = ["ModeToggleUnavailableBattlePet"];
        }
    }

    static readonly Stylesheet ModeToggleStylesheet = new Stylesheet(
        [
            new(
                ".ModeToggleBase",
                new()
                {
                    Anchor = Anchor.TopLeft,
                    Flow = Flow.Horizontal,
                    Size = new Size(70, 28),
                    Padding = new(2),
                    BackgroundColor = new Color("Window"),
                    Margin = new(2, 5),
                }
            ),
            new(".ModeToggleUnavailableMinion",
                new()
                {
                    Anchor = Anchor.MiddleLeft,
                    Flow = Flow.Vertical,
                    Size = new Size(32, 15),
                    BackgroundColor = new("ModeToggleInactive"),
                    BorderRadius = 6,
                    IsAntialiased = false,
                    RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
                    ShadowSize = new(5),
                    ShadowInset = 8,
                    Padding = new(2),
                }),
            new(".ModeToggleUnavailableBattlePet",
                new()
                {
                    Anchor = Anchor.MiddleLeft,
                    Flow = Flow.Vertical,
                    Size = new Size(32, 15),
                    BackgroundColor = new("ModeToggleInactive"),
                    BorderRadius = 6,
                    IsAntialiased = false,
                    RoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
                    ShadowSize = new(5),
                    ShadowInset = 8,
                    Padding = new(2),
                }),
            new(".BattlePetModeActive",
                new()
                {
                    Anchor = Anchor.MiddleLeft,
                    Flow = Flow.Vertical,
                    Size = new Size(32, 15),
                    BackgroundColor = new("Titlebar.BattlePet"),
                    BorderRadius = 6,
                    IsAntialiased = true,
                    RoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
                    ShadowSize = new(5),
                    ShadowInset = 8,
                    Padding = new(2),
                }),
            new(".BattlePetModeActive:hover",
                new()
                {
                    BackgroundColor = new("Titlebar.BattlePet:Dark"),
                }),
            new(".MinionModeActive",
                new()
                {
                    Anchor = Anchor.MiddleLeft,
                    Flow = Flow.Vertical,
                    Size = new Size(32, 15),
                    BackgroundColor = new("Titlebar.Minion"),
                    BorderRadius = 6,
                    IsAntialiased = true,
                    RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
                    ShadowSize = new(5),
                    ShadowInset = 8,
                    Padding = new(2),
                }),
            new(".MinionModeActive:hover",
                new()
                {
                    BackgroundColor = new("Titlebar.Minion:Dark"),
                }),
        ]);
}
