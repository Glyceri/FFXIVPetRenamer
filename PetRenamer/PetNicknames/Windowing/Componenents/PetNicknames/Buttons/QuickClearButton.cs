using Dalamud.Interface;
using ImGuiNET;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Buttons;

internal class QuickClearButton : QuickSquareButton
{
    bool lastState = false;

    public QuickClearButton()
    {
        Tooltip = "Hold Left Ctrl + Left Shift to delete an entry";
        NodeValue = FontAwesomeIcon.Times.ToIconString();
    }

    protected override void ButtonClicked()
    {
        if (lastState) return;
        base.ButtonClicked();
    }

    protected override void OnDraw(ImDrawListPtr drawList)
    {
        if (!ImGui.IsKeyDown(ImGuiKey.LeftCtrl) || !ImGui.IsKeyDown(ImGuiKey.LeftShift))
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
