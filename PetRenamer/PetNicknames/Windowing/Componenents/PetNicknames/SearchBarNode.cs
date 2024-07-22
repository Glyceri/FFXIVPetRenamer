using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.TranslatorSystem;
using System;
using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class SearchBarNode : RenameTitleNode
{
    public string InputFieldvalue = "";

    protected string InternalID = "SEARCH";

    public Action<string?>? OnSave;

    public SearchBarNode(in DalamudServices services, string label, string? text) : base(in services, label, text ?? Translator.GetLine("..."))
    {

    }

    public void ClearSearchbar()
    {
        InputFieldvalue = "";
    }

    public void SetInputFieldValue(string value)
    {
        InputFieldvalue = value ?? string.Empty;
    }

    protected virtual void OnSearch()
    {
        OnSave?.Invoke(InputFieldvalue);
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        ImGui.SetCursorScreenPos(TextNode.Bounds.ContentRect.BottomLeft - new Vector2(0, 18 * ImGuiHelpers.GlobalScale));
        ImGui.SetNextItemWidth(TextNode.Bounds.ContentRect.Width);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Color("SearchBarBackground").ToUInt());
        if (ImGui.InputText($"##RenameField_{this.GetFullNodePath()}_{Label}", ref InputFieldvalue, PluginConstants.ffxivNameSize, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.None))
        {
            OnSearch();
        }
        ImGui.PopStyleColor();
    }

    public bool Valid(string input)
    {
        if (InputFieldvalue.IsNullOrWhitespace()) return true;

        return input.Contains(InputFieldvalue, StringComparison.InvariantCultureIgnoreCase);
    }
}
