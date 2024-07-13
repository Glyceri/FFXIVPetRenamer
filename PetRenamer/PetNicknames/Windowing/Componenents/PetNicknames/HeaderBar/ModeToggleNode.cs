using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Enums;
using System;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.HeaderBar;

internal class ModeToggleNode : Node
{
    readonly Node CompanionNode;
    readonly Node BattlePetNode;

    public event Action<PetWindowMode>? OnModeChange;

    readonly DalamudServices DalamudServices;

    public ModeToggleNode(in DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;
        Stylesheet = ModeToggleStylesheet;
        ClassList = ["ModeToggleBase"];

        ChildNodes = [
           CompanionNode = new()
           {
               ClassList = ["ModeToggleUnavailableMinion"]
           },
            BattlePetNode = new()
            {
                ClassList = ["BattlePetModeActive"]
            },
        ];

        CompanionNode.OnMouseUp += _ => DalamudServices.Framework.Run(() => OnModeChange?.Invoke(PetWindowMode.Minion));
        BattlePetNode.OnMouseUp += _ => DalamudServices.Framework.Run(() => OnModeChange?.Invoke(PetWindowMode.BattlePet));
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
                    Anchor = Anchor.MiddleLeft,
                    Size = new Size(64, 15),
                    BackgroundColor = new Color("Window"),
                    Margin = new EdgeSize(0, 5),
                }
            ),
            new(".ModeToggleUnavailableMinion",
                new()
                {
                    Anchor = Anchor.MiddleLeft,
                    Size = new Size(32, 15),
                    BackgroundColor = new("ModeToggleInactive"),
                    BorderRadius = 6,
                    RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
                }),
            new(".ModeToggleUnavailableBattlePet",
                new()
                {
                    Anchor = Anchor.MiddleLeft,
                    Size = new Size(32, 15),
                    BackgroundColor = new("ModeToggleInactive"),
                    BorderRadius = 6,
                    RoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
                }),
            new(".BattlePetModeActive",
                new()
                {
                    Anchor = Anchor.MiddleLeft,
                    Size = new Size(32, 15),
                    BackgroundColor = new("Titlebar.BattlePet"),
                    BorderRadius = 6,
                    RoundedCorners = RoundedCorners.TopRight | RoundedCorners.BottomRight,
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
                    RoundedCorners = RoundedCorners.TopLeft | RoundedCorners.BottomLeft,
                }),
            new(".MinionModeActive:hover",
                new()
                {
                    BackgroundColor = new("Titlebar.Minion:Dark"),
                }
            ),
        ]
    );
}
