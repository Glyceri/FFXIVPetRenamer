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
    protected string inputFieldvalue = "";

    protected string InternalID = "SEARCH";

    public Action<string?>? OnSave;

    public SearchBarNode(in DalamudServices services, string label, string? text) : base(in services, label, text ?? Translator.GetLine("..."))
    {

    }

    public void ClearSearchbar()
    {
        inputFieldvalue = "";
    }

    public void SetInputFieldValue(string value)
    {
        inputFieldvalue = value ?? string.Empty;
    }

    protected virtual void OnSearch()
    {
        OnSave?.Invoke(inputFieldvalue);
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        ImGui.SetCursorScreenPos(TextNode.Bounds.ContentRect.TopLeft - new Vector2(0, Node.ScaleFactor * 3));
        ImGui.SetNextItemWidth(TextNode.Bounds.ContentRect.Width);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Color("Window.Background").ToUInt());
        if (ImGui.InputText($"##RenameField_{InternalID}", ref inputFieldvalue, PluginConstants.ffxivNameSize, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.None))
        {
            OnSearch();
        }
        ImGui.PopStyleColor();
    }

    public bool Valid(string input)
    {
        if (inputFieldvalue.IsNullOrWhitespace()) return true;

        return input.Contains(inputFieldvalue, StringComparison.InvariantCultureIgnoreCase);
    }
}
