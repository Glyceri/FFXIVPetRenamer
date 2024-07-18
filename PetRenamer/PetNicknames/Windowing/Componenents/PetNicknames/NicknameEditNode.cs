using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class NicknameEditNode : SearchBarNode
{
    public readonly QuickButton EditButton;
    public readonly QuickButton ClearButton;

    bool editMode = false;

    bool shouldBeVisible = true;

    IPetSheetData? ActivePet;
    string? CurrentValue = null;

    public NicknameEditNode(in DalamudServices services, string label, string? text) : base(in services, label, text ?? Translator.GetLine("..."))
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
        SetText(CurrentValue ?? Translator.GetLine("..."));
        SetInputFieldValue(CurrentValue ?? string.Empty);
    }

    protected override void OnSearch()
    {
        EditClicked();
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        EditButton.Style.IsVisible = shouldBeVisible;
        ClearButton.Style.IsVisible = shouldBeVisible;

        if (editMode) 
        {
            InternalID = ActivePet?.BaseSingular ?? string.Empty;
            base.OnDraw(drawList); 
        }
    }
}
