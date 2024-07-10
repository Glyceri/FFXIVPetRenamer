using ImGuiNET;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using System;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class NicknameEditNode : RenameTitleNode
{
    readonly QuickButton EditButton;
    readonly QuickButton ClearButton;
    readonly Node ButtonHolder;

    bool editMode = false;

    bool shouldBeVisible = true;

    IPetSheetData? ActivePet;
    string? CurrentValue = null;
    string inputFieldvalue = "";

    public Action<string?>? OnSave;

    public NicknameEditNode(string label, string text) : base(label, text)
    {
        ButtonHolder = new Node()
        {
            Style = new Style()
            {
                Flow = Flow.Horizontal,
                Anchor = Anchor.TopRight,
            },
            ChildNodes = 
            [
                EditButton = new QuickButton($"{Translator.GetLine("PetRenameNode.Edit")}"),
                ClearButton = new QuickButton($"{Translator.GetLine("PetRenameNode.Clear")}"),
            ]
        };

        ClearButton.Clicked += ClearClicked;
        EditButton.Clicked += EditClicked;

        UnderlineNode.AppendChild(ButtonHolder);
    }

    public void SetPet(string? customName, in IPetSheetData? activePet)
    {
        ActivePet = activePet;
        CurrentValue = customName;
        shouldBeVisible = activePet != null;


        StopEditMode();
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
        base.OnDraw(drawList);

        ButtonHolder.Style.IsVisible = shouldBeVisible;
        

       if (editMode)
        {
            ImGui.SetCursorScreenPos(TextNode.Bounds.ContentRect.TopLeft);
            ImGui.SetNextItemWidth(TextNode.Bounds.ContentRect.Width);
            if (ImGui.InputText($"##RenameField_{ActivePet?.Model}", ref inputFieldvalue, PluginConstants.ffxivNameSize, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.None))
            {
                EditButton.Clicked?.Invoke();
            }
        }
    }
}
