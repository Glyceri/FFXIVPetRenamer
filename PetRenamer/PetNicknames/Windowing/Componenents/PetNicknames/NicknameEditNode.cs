using Dalamud.Interface;
using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using PetRenamer.PetNicknames.TranslatorSystem;
using PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class NicknameEditNode : SearchBarNode
{
    public readonly QuickSquareButton EditButton;
    public readonly QuickSquareButton ClearButton;

    bool editMode = false;

    bool shouldBeVisible = true;

    IPetSheetData? ActivePet;
    string? CurrentValue = null;

    public NicknameEditNode(in DalamudServices services, string label, string? text) : base(in services, label, text ?? Translator.GetLine("..."))
    {
        EditButton = new QuickSquareButton()
        {
            Style = new Style()
            {
                FontSize = 7,
                Size = new Size(40, 14),
                Anchor = Anchor.TopRight,
            }
        };
        ClearButton = new QuickSquareButton()
        {
            Style = new Style()
            {
                FontSize = 7,
                Size = new Size(40, 14),
                Anchor = Anchor.TopRight,
            }
        };

        ClearButton.OnClick += ClearClicked;
        EditButton.OnClick += EditClicked;

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
            OnSave?.Invoke(InputFieldvalue);
            StopEditMode();
        }
        else StartEditMode();
    }

    void StartEditMode()
    {
        EditButton.NodeValue = FontAwesomeIcon.Save.ToIconString();
        ClearButton.NodeValue = FontAwesomeIcon.Times.ToIconString();

        EditButton.Tooltip = Translator.GetLine("PetRenameNode.Save");
        ClearButton.Tooltip = Translator.GetLine("PetRenameNode.Cancel");

        SetText("");
        editMode = true;
    }

    void StopEditMode()
    {
        editMode = false;

        ClearButton.NodeValue = FontAwesomeIcon.Eraser.ToIconString();
        EditButton.NodeValue = FontAwesomeIcon.Edit.ToIconString();
        EditButton.Tooltip = Translator.GetLine("PetRenameNode.Edit");
        ClearButton.Tooltip = Translator.GetLine("PetRenameNode.Clear");

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
