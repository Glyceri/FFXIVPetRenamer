﻿using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetListWindow;

internal partial class PetListWindow
{
    protected override string Title { get; } = "Pet List";
    protected override string ID { get; } = "Pet List Window";

    protected override Vector2 MinSize { get; } = new Vector2(550, 240);
    protected override Vector2 MaxSize { get; } = new Vector2(550, 997);
    protected override Vector2 DefaultSize { get; } = new Vector2(550, 240);
    protected override bool HasModeToggle { get; } = true;
}
