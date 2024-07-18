using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Windows.ColourEditorWindow;

internal class ColourEditorWindow : PetWindow
{

    protected override Vector2 MinSize { get; } = new Vector2(500, 600);
    protected override Vector2 MaxSize { get; } = new Vector2(500, 600);
    protected override Vector2 DefaultSize { get; } = new Vector2(500, 600);
    protected override bool HasModeToggle { get; } = false;
    protected override bool HasExtraButtons { get; } = false;

    protected override string Title { get; } = Translator.GetLine("ColourEditorWindow.Title");
    protected override string ID { get; } = "ColourEditorWindow";

    public ColourEditorWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration) : base(windowHandler, dalamudServices, configuration, "ColourEditorWindow")
    {
        IsOpen = false;
    }

    public override void OnDraw()
    {
        
    }
}
