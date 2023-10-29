using Dalamud.Game.Text;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Theming;
using PetRenamer.Windows.Attributes;
using System.Numerics;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
[ModeTogglePetWindow]
internal class ThemeEditorWindow : PetWindow
{
    Vector2 baseSize = new Vector2(800, 500);

    public ThemeEditorWindow() : base("Pet Nicknames Theme Editor")
    {
        Size = baseSize;
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = baseSize,
            MaximumSize = new Vector2(9999, 9999)
        };
    }

    Vector4 refColour;

    bool hasChanges = false;

    public override void OnWindowOpen() => ResetAllThemes();

    void ResetAllThemes()
    {
        ThemeHandler.FillTheme(ThemeHandler.baseThemeCustomTemporary, PluginLink.Configuration.CustomBaseTheme);
        ThemeHandler.FillTheme(ThemeHandler.greenThemeCustomTemporary, PluginLink.Configuration.CustomGreenTheme);
        ThemeHandler.FillTheme(ThemeHandler.redThemeCustomTemporary, PluginLink.Configuration.CustomRedTheme);
    }

    void ResetAllThemesBase()
    {
        ThemeHandler.FillTheme(ThemeHandler.baseThemeCustomTemporary, ThemeHandler.baseTheme);
        ThemeHandler.FillTheme(ThemeHandler.greenThemeCustomTemporary, ThemeHandler.greenTheme);
        ThemeHandler.FillTheme(ThemeHandler.redThemeCustomTemporary, ThemeHandler.redTheme);
    }

    public override void OnDraw()
    {
        Theme currentTheme = ThemeHandler.ActiveTheme;
        Theme customThemeTemporary = petMode == PetMode.Normal ? ThemeHandler.baseThemeCustomTemporary : petMode == PetMode.BattlePet ? ThemeHandler.greenThemeCustomTemporary : ThemeHandler.redThemeCustomTemporary;
        Theme customTheme = petMode == PetMode.Normal ? PluginLink.Configuration.CustomBaseTheme : petMode == PetMode.BattlePet ? PluginLink.Configuration.CustomGreenTheme : PluginLink.Configuration.CustomRedTheme;

        DrawLeftSide(ref currentTheme, ref customThemeTemporary, ref customTheme);  

        ThemeHandler.SetTheme(currentTheme);

        SameLinePretendSpace();

        if (BeginListBox($"##[themeBox2]{internalCounter++}", new Vector2(ContentAvailableX, ContentAvailableY)))
        {
            ThemeHandler.SetTheme(ThemeHandler.BlackWhite);

            if (BeginListBox($"##{internalCounter++}", new Vector2(ContentAvailableX, BarSizePadded)))
            {
                Label("This Side Will ALWAYS Display The Custom Theme", new Vector2(ContentAvailableX, BarSize));
                ImGui.EndListBox();
            }
            NewLine();
                if (BeginListBox($"##[themeBox12]{internalCounter++}", new Vector2(ContentAvailableX, 181)))
            {
                ThemeHandler.SetTheme(customThemeTemporary);
                Label("This is how a basic Label Looks", new Vector2(ContentAvailableX, BarSize));
                Button("This is how a basic Button looks", new Vector2(ContentAvailableX, BarSize));
                OverrideLabel("This is how a basic text input box/contrast label looks", new Vector2(ContentAvailableX, BarSize));
                PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
                ImGui.Text("This is how basic text looks");
                PushStyleColor(ImGuiCol.Text, StylingColours.alternativeText);
                ImGui.Text("This is how alternative text looks");
                PushStyleColor(ImGuiCol.Text, StylingColours.tooltipText);
                ImGui.Text("This is how tooltip text looks");

                if (BeginListBox($"##[themeBox13]{internalCounter++}", new Vector2(ContentAvailableX, BarSize)))
                {
                    PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
                    ImGui.Text("This is how a basic background panel looks");
                    ImGui.EndListBox();
                }

                ImGui.EndListBox();
            }

            NewLine();

            ThemeHandler.SetTheme(ThemeHandler.BlackWhite);
            if (BeginListBox($"##[themeBox3]{internalCounter++}", new Vector2(ContentAvailableX, 190)))
            {
                ThemeHandler.SetTheme(customThemeTemporary);
                Label($"Pet Rename Window##{internalCounter++}", new Vector2(ContentAvailableX, BarSize));
                PluginLink.WindowHandler.GetWindow<PetRenameWindow>()?.Draw();

                ImGui.EndListBox();
            }

            NewLine();
            ThemeHandler.SetTheme(ThemeHandler.BlackWhite);
            if (BeginListBox($"##[themeBox12]{internalCounter++}", new Vector2(ContentAvailableX, 178)))
            {
                ThemeHandler.SetTheme(customThemeTemporary);

                Label($"Pet Config Window##{internalCounter++}", new Vector2(ContentAvailableX, BarSize));
                PluginLink.WindowHandler.GetWindow<ConfigWindow>()?.Draw();

                ImGui.EndListBox();
            }
            ImGui.EndListBox();
        }

        SetTheme();
    }

    void DrawLeftSide(ref Theme currentTheme, ref Theme customThemeTemporary, ref Theme customTheme)
    {
        PushStyleColor(ImGuiCol.Text, ThemeHandler.ActiveTheme.defaultText);

        if (BeginListBox($"##[themeBox]{internalCounter++}", new Vector2(ContentAvailableX / 2, ContentAvailableY)))
        {
            if (!PluginLink.Configuration.newUseCustomTheme)
            {
                if (BeginListBox($"##{internalCounter++}", new Vector2(ContentAvailableX, BarSizePadded)))
                {
                    OverrideLabel($"Please Enable the Custom Theme Setting in the Settings Window##{internalCounter++}", new Vector2(ContentAvailableX - BarSize - SpaceSize- FramePaddingX, BarSize));
                    SameLinePretendSpace();
                    if (Button($"{SeIconChar.MouseWheel.ToIconString()}##{internalCounter++}", new Vector2(BarSize, BarSize), "Open Settings Window"))
                        PluginLink.WindowHandler.OpenWindow<ConfigWindow>();
                    ImGui.EndListBox();
                }

                ImGui.EndListBox();
                return;
            }

            if (hasChanges) ThemeHandler.SetTheme(ThemeHandler.redTheme);
            else ThemeHandler.SetTheme(ThemeHandler.BlackWhite);

            if (BeginListBox($"##{internalCounter++}", new Vector2(ContentAvailableX, BarSizePadded)))
            {
                if (Button($"{SeIconChar.BoxedLetterR.ToIconString()}##{internalCounter++}", "Reset ALL Colours")) ResetAllThemes();
                SameLinePretendSpace2();
                if (Button($"{SeIconChar.BoxedLetterB.ToIconString()}##{internalCounter++}", "Reset ALL to Base Colour")) ResetAllThemesBase();
                SameLinePretendSpace2();
                if (Button($"{SeIconChar.BoxedLetterS.ToIconString()}##{internalCounter++}", "Save Current Theme"))
                {
                    ThemeHandler.FillTheme(customTheme, customThemeTemporary);
                    PluginLink.Configuration.Save();
                }

                ImGui.EndListBox();
            }
            hasChanges = false;
            ThemeHandler.SetTheme(ThemeHandler.BlackWhite);

            refColour = customThemeTemporary.basicLabelColour;
            if (DrawColour(ref refColour, customTheme.basicLabelColour, currentTheme.basicLabelColour, "Basic Label Background")) customThemeTemporary.basicLabelColour = refColour;

            refColour = customThemeTemporary.button;
            if (DrawColour(ref refColour, customTheme.button, currentTheme.button, "Basic Button")) customThemeTemporary.button = refColour;

            refColour = customThemeTemporary.buttonHovered;
            if (DrawColour(ref refColour, customTheme.buttonHovered, currentTheme.buttonHovered, "Basic Button Hovered")) customThemeTemporary.buttonHovered = refColour;

            refColour = customThemeTemporary.buttonPressed;
            if (DrawColour(ref refColour, customTheme.buttonPressed, currentTheme.buttonPressed, "Basic Button Pressed")) customThemeTemporary.buttonPressed = refColour;

            refColour = customThemeTemporary.buttonAlternative;
            if (DrawColour(ref refColour, customTheme.buttonAlternative, currentTheme.buttonAlternative, "Alternative Button")) customThemeTemporary.buttonAlternative = refColour;

            refColour = customThemeTemporary.buttonAlternativeHovered;
            if (DrawColour(ref refColour, customTheme.buttonAlternativeHovered, currentTheme.buttonAlternativeHovered, "Alternative Button Hovered")) customThemeTemporary.buttonAlternativeHovered = refColour;

            refColour = customThemeTemporary.buttonAlternativePressed;
            if (DrawColour(ref refColour, customTheme.buttonAlternativePressed, currentTheme.buttonAlternativePressed, "Alternative Button Pressed")) customThemeTemporary.buttonAlternativePressed = refColour;

            refColour = customThemeTemporary.defaultText;
            if (DrawColour(ref refColour, customTheme.defaultText, currentTheme.defaultText, "Basic Text")) customThemeTemporary.defaultText = refColour;

            refColour = customThemeTemporary.alternativeText;
            if (DrawColour(ref refColour, customTheme.alternativeText, currentTheme.alternativeText, "Alternative Text")) customThemeTemporary.alternativeText = refColour;

            refColour = customThemeTemporary.tooltipText;
            if (DrawColour(ref refColour, customTheme.tooltipText, currentTheme.tooltipText, "Tooltip Text")) customThemeTemporary.tooltipText = refColour;

            refColour = customThemeTemporary.panelColour;
            if (DrawColour(ref refColour, customTheme.panelColour, currentTheme.panelColour, "Panel Background")) customThemeTemporary.panelColour = refColour;

            refColour = customThemeTemporary.ipcLabelColour;
            if (DrawColour(ref refColour, customTheme.ipcLabelColour, currentTheme.ipcLabelColour, "Ipc Panel Colour")) customThemeTemporary.ipcLabelColour = refColour;

            refColour = customThemeTemporary.panelSubColour;
            if (DrawColour(ref refColour, customTheme.panelSubColour, currentTheme.panelSubColour, "Panel Sub Colour")) customThemeTemporary.panelSubColour = refColour;

            refColour = customThemeTemporary.titleBgActive;
            if (DrawColour(ref refColour, customTheme.titleBgActive, currentTheme.titleBgActive, "Title Background Active")) customThemeTemporary.titleBgActive = refColour;

            refColour = customThemeTemporary.tileBgCollapsed;
            if (DrawColour(ref refColour, customTheme.tileBgCollapsed, currentTheme.tileBgCollapsed, "Title Background Collapsed")) customThemeTemporary.tileBgCollapsed = refColour;

            ImGui.EndListBox();
        }
    }


    bool DrawColour(ref Vector4 colourEditor, Vector4 resetColour, Vector4 baseColour, string title)
    {
        bool outcome = false;
        if (colourEditor != resetColour) 
        { 
            hasChanges = true;
            ThemeHandler.SetTheme(ThemeHandler.redTheme);
        }
        else
        {
            ThemeHandler.SetTheme(ThemeHandler.BlackWhite);
        }
        if (BeginListBox($"##{internalCounter++}", new Vector2(ContentAvailableX, BarSizePadded)))
        {
            if (Button($"{SeIconChar.BoxedLetterR.ToIconString()}##{internalCounter++}", "Reset Colour"))
            {
                colourEditor = resetColour;
                ImGui.EndListBox();
                return true;
            }
            SameLinePretendSpace();
            if (Button($"{SeIconChar.BoxedLetterB.ToIconString()}##{internalCounter++}", "Reset To Base Colour"))
            {
                colourEditor = baseColour;
                ImGui.EndListBox();
                return true;
            }
            SameLinePretendSpace();
            outcome = ImGui.ColorEdit4($"{title}##{internalCounter++}", ref colourEditor);
            ImGui.EndListBox();
        }
        return outcome;
    }
}
