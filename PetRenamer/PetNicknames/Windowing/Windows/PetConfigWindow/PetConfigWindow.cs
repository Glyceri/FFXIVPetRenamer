using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Base;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows.PetConfigWindow;

internal class PetConfigWindow : PetWindow
{
    protected override string ID { get; } = "Configuration";
    protected override Vector2 MinSize { get; } = new Vector2(400, 500);
    protected override Vector2 MaxSize { get; } = new Vector2(400, 500);
    protected override Vector2 DefaultSize { get; } = new Vector2(400, 500);
    protected override bool HasModeToggle { get; } = true;
    protected override string Title { get; } = "Configuration";

    public PetConfigWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration) : base(windowHandler, dalamudServices, configuration, "Configuration")
    {
    }

    public override void OnDraw()
    {
        
    }
}
