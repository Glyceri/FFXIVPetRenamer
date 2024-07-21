using PetRenamer.PetNicknames.ColourProfiling;
using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Base.Style;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;
using System.Collections.Generic;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Windows.ColourEditorWindow;

internal class ColourEditorWindow : PetWindow
{

    protected override Vector2 MinSize { get; } = new Vector2(400, 300);
    protected override Vector2 MaxSize { get; } = new Vector2(400, 1200);
    protected override Vector2 DefaultSize { get; } = new Vector2(400, 600);
    protected override bool HasModeToggle { get; } = false;
    protected override bool HasExtraButtons { get; } = false;

    protected override string Title { get; } = Translator.GetLine("ColourEditorWindow.Title");
    protected override string ID { get; } = "ColourEditorWindow";

    readonly IColourProfileHandler ColourProfileHandler;

    IColourProfile? windowActiveProfile;

    List<ColourSettingsNode> colourSettingNodes = new List<ColourSettingsNode>();

    bool didActivate = false;

    readonly Node ColourHolderNode;

    public ColourEditorWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, in IColourProfileHandler colourProfileHandler) : base(windowHandler, dalamudServices, configuration, "ColourEditorWindow")
    {
        IsOpen = true;

        ColourProfileHandler = colourProfileHandler;

        ContentNode.ChildNodes = 
        [
            ColourHolderNode = new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical
                }
            }
        ];

        IColourProfile defaultProfile = WindowStyles.DefaultColourProfile;
        IColourProfile? activeProfile = colourProfileHandler.GetActive();

        if (activeProfile != null)
        {
            SetActiveProfile(activeProfile);
        }

        Register("UnderlineColour");
        Register("UnderlineColour:Fade");
        Register("Window.Background");
        Register("Window.BackgroundLight");
        Register("SearchBarBackground");
        Register("Window.TextOutline");
        Register("Window.TextOutlineButton");
        Register("Window.Text");
        Register("Window.TextLight");
        Register("Window.TitleBarBorder");
        Register("WindowBorder:Active");
        Register("WindowBorder:Inactive");
    }

    void Register(string name)
    {
        ColourSettingsNode node = new ColourSettingsNode(in Configuration, name, () =>
        {
            didActivate = true;
        });
        colourSettingNodes.Add(node);
        ColourHolderNode.ChildNodes.Add(node);
    }

    public void SetActiveProfile(IColourProfile profile)
    {
        windowActiveProfile = profile;
    }

    public override void OnDraw()
    {
        ColourHolderNode.Style.IsVisible = windowActiveProfile != null;

        if (windowActiveProfile == null) return;

        foreach (ColourSettingsNode node in  colourSettingNodes)
        {
            node.UpdateProfile(windowActiveProfile);
        }
    }
}
