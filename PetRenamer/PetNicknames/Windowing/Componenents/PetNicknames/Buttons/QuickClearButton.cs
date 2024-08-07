﻿using Dalamud.Interface;
using ImGuiNET;
using PetRenamer.PetNicknames.TranslatorSystem;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;

internal class QuickClearButton : QuickSquareButton
{
    bool lastState = false;

    public bool Locked { get; set; } = false;

    public QuickClearButton()
    {
        Tooltip = Translator.GetLine("ClearButton.Label");
        NodeValue = FontAwesomeIcon.Times.ToIconString();
        TagsList.Add("fakeDisabled");
    }

    protected override void ButtonClicked()
    {
        if (Locked) return;
        if (lastState) return;
        base.ButtonClicked();
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        if (!ImGui.IsKeyDown(ImGuiKey.LeftCtrl) || !ImGui.IsKeyDown(ImGuiKey.LeftShift) || Locked)
        {
            if (lastState == false)
            {
                lastState = true;
                TagsList.Add("fakeDisabled");
            }
        }
        else
        {
            if (lastState == true)
            {
                lastState = false;
                TagsList.Remove("fakeDisabled");
            }
        }
    }
}
