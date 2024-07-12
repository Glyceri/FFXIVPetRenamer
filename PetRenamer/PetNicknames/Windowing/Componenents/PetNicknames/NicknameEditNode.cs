using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using System;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class NicknameEditNode : RenameTitleNode
{
    public readonly QuickButton EditButton;
    public readonly QuickButton ClearButton;

    bool editMode = false;

    bool shouldBeVisible = true;

    IPetSheetData? ActivePet;
    string? CurrentValue = null;
    string inputFieldvalue = "";

    public Action<string?>? OnSave;

    public NicknameEditNode(in DalamudServices services, string label, string? text) : base(in services, label, text ?? "...")
    {
        EditButton = new QuickButton(in DalamudServices, $"{Translator.GetLine("PetRenameNode.Edit")}")
        {
            Style = new Style()
            {
                FontSize = 7,
                Size = new Size(40, 14),
                Anchor = Anchor.TopRight,
            }
        };
        ClearButton = new QuickButton(in DalamudServices, $"{Translator.GetLine("PetRenameNode.Clear")}")
        {
            Style = new Style()
            {
                FontSize = 7,
                Size = new Size(40, 14),
                Anchor = Anchor.TopRight,
            }
        };

        ClearButton.Clicked += ClearClicked;
        EditButton.Clicked += EditClicked;

        UnderlineNode.AppendChild(ClearButton);
        UnderlineNode.AppendChild(EditButton);

        if (text == null)
        {
            StartEditMode();
        }
    }

    public void SetPet(string? customName, in IPetSheetData? activePet)
    {
        ActivePet = activePet;
        CurrentValue = customName;
        shouldBeVisible = activePet != null;

        StopEditMode();

        if (customName == null && activePet != null)
        {
            StartEditMode();
        }
    }

    void ClearClicked()
    {
        if (editMode) StopEditMode();
        else OnSave?.Invoke(null);
    }

    void EditClicked()
    {
        if (editMode)
        {
            OnSave?.Invoke(inputFieldvalue);
            StopEditMode();
        }
        else StartEditMode();
    }

    void StartEditMode()
    {
        EditButton.SetText($"{Translator.GetLine("PetRenameNode.Save")}");
        ClearButton.SetText($"{Translator.GetLine("PetRenameNode.Cancel")}");
        SetText("");
        editMode = true;
    }

    void StopEditMode()
    {
        editMode = false;
        ClearButton.SetText($"{Translator.GetLine("PetRenameNode.Clear")}");
        EditButton.SetText($"{Translator.GetLine("PetRenameNode.Edit")}");
        SetText(CurrentValue ?? "...");
        inputFieldvalue = CurrentValue ?? string.Empty;
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        EditButton.Style.IsVisible = shouldBeVisible;
        ClearButton.Style.IsVisible = shouldBeVisible;

        if (editMode)
        {
            ImGui.SetCursorScreenPos(TextNode.Bounds.ContentRect.TopLeft - new Vector2(0, Node.ScaleFactor * 3));
            ImGui.SetNextItemWidth(TextNode.Bounds.ContentRect.Width);
            if (ImGui.InputText($"##RenameField_{ActivePet?.Model}", ref inputFieldvalue, PluginConstants.ffxivNameSize, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.None))
            {
                EditButton.Clicked?.Invoke();
            }
        }
    }
}
