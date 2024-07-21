using PetRenamer.PetNicknames.ColourProfiling;
using PetRenamer.PetNicknames.ColourProfiling.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Base.Style;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
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

    List<ColourSettingsNode> colourSettingNodes = new List<ColourSettingsNode>();

    bool didActivate = false;

    readonly SettingsHolderNode PresetList;
    readonly SettingsHolderNode ColourHolderNode;

    readonly QuickSquareButton QuickButton;

    public ColourEditorWindow(in WindowHandler windowHandler, in DalamudServices dalamudServices, in Configuration configuration, in IColourProfileHandler colourProfileHandler) : base(windowHandler, dalamudServices, configuration, "ColourEditorWindow")
    {
        IsOpen = true;

        ColourProfileHandler = colourProfileHandler;


        ContentNode.Overflow = false;

        ContentNode.ChildNodes = 
        [
            new Node()
            {
                Style = new Style()
                {
                    Flow = Flow.Vertical,
                },
                ChildNodes = [
                    QuickButton = new QuickSquareButton(),
                    PresetList = new SettingsHolderNode(in Configuration, Translator.GetLine("ColourSettings.PresetListHeader")),
                    ColourHolderNode = new SettingsHolderNode(in Configuration, Translator.GetLine("ColourSettings.Header"))
                    {
                        Style = new Style()
                        {
                            Flow = Flow.Vertical
                        }
                    },
                ]
            }
        ];

        QuickButton.OnClick += () =>
        {
            ColourProfileHandler.AddColourProfile(new ColourProfile("Testing", "Glyceri", WindowStyles.DefaultColourProfile.Colours.ToList()));
        };

        Register("Outline");
        Register("Outline:Fade");
        Register("Window.Background");
        Register("Window.BackgroundLight");
        Register("SearchBarBackground");
        Register("Window.TextOutline");
        Register("Window.TextOutlineButton");
        Register("Window.Text");
        Register("Window.TextLight");
        Register("WindowBorder:Active");
        Register("WindowBorder:Inactive");
        Register("Button.Background");
        Register("Button.Background:Hover");
        Register("Button.Background:Inactive");
        Register("FlareImageColour");

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

        int activeIndex = ColourProfileHandler.GetActiveAsSerialized();


        int index = -1;
        AddColourprofile(WindowStyles.DefaultColourProfile, index, index == activeIndex);
        index++;

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
    }

    void AddColourprofile(IColourProfile cProfile, int index, bool active)
    {
        ColourProfileConfig cConfig = new ColourProfileConfig(in Configuration, cProfile.Name, cProfile.Author, index, active, (value) => {
            DalamudServices.Framework.Run(() => 
            { 
                ColourProfileHandler.SetActiveProfile(cProfile); 
                WindowHandler?.GetWindow<ColourEditorWindow>()?.OnPresetListChanged(); 
            });
        });
        PresetList.ContentNode.ChildNodes.Add(cConfig);
    }

    public override void OnDraw()
    {
        int activeIndex = ColourProfileHandler.GetActiveAsSerialized();

        ColourHolderNode.Style.IsVisible = activeIndex != -1;

        if (didActivate)
        {
            didActivate = false;
            ColourProfileHandler.Refresh();

        }
    }
}
