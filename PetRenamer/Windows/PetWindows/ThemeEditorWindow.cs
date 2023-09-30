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
        IsOpen = true;
        Size = baseSize;
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = baseSize,
            MaximumSize = new Vector2(9999, 9999)
        };
    }

    public override void OnDraw()
    {
        Theme currentTheme = ThemeHandler.ActiveTheme;
        Theme customTheme = petMode == PetMode.Normal ? ThemeHandler.baseThemeCustom : petMode == PetMode.BattlePet ? ThemeHandler.greenThemeCustom : ThemeHandler.redThemeCustom;

        ThemeHandler.SetTheme(ThemeHandler.BlackWhite);
        PushStyleColor(ImGuiCol.Text, ThemeHandler.ActiveTheme.defaultText);

        if (BeginListBox($"##[themeBox]{internalCounter++}", new Vector2(ContentAvailableX / 2, ContentAvailableY)))
        {
            Vector4 basicLabelColour = customTheme.basicLabelColour;
            ImGui.PushItemWidth(150);
            if (ImGui.ColorPicker4(string.Empty, ref basicLabelColour))
                customTheme.basicLabelColour = basicLabelColour;
            
            ImGui.PopItemWidth();

            ImGui.EndListBox();
        }

        ThemeHandler.SetTheme(customTheme);

        SameLinePretendSpace();

        if (!BeginListBox($"##[themeBox2]{internalCounter++}", new Vector2(ContentAvailableX, ContentAvailableY))) return;

        ThemeHandler.SetTheme(ThemeHandler.BlackWhite);
        if (BeginListBox($"##[themeBox12]{internalCounter++}", new Vector2(ContentAvailableX, 161)))
        {
            ThemeHandler.SetTheme(customTheme);
            Label("This is how a basic Label Looks", new Vector2(ContentAvailableX, BarSize));
            Button("This is how a basic Button looks", new Vector2(ContentAvailableX, BarSize));
            OverrideLabel("This is how a basic text input box/contrast label looks", new Vector2(ContentAvailableX, BarSize));
            PushStyleColor(ImGuiCol.Text, StylingColours.defaultText);
            ImGui.Text("This is how basic text looks");
            PushStyleColor(ImGuiCol.Text, StylingColours.alternativeText);
            ImGui.Text("This is how alternative text looks");

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
            ThemeHandler.SetTheme(customTheme);
            Label($"Pet Rename Window##{internalCounter++}", new Vector2(ContentAvailableX, BarSize));
            PluginLink.WindowHandler.GetWindow<PetRenameWindow>()?.Draw();

            ImGui.EndListBox();
        }

        NewLine();
        ThemeHandler.SetTheme(ThemeHandler.BlackWhite);
        if (BeginListBox($"##[themeBox12]{internalCounter++}", new Vector2(ContentAvailableX, 178)))
        {
            ThemeHandler.SetTheme(customTheme);

            Label($"Pet Config Window##{internalCounter++}", new Vector2(ContentAvailableX, BarSize));
            PluginLink.WindowHandler.GetWindow<ConfigWindow>()?.Draw();

            ImGui.EndListBox();
        }

        ThemeHandler.SetTheme(currentTheme);
    }
}
