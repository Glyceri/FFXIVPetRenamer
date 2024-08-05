using ImGuiNET;
using PetRenamer.PetNicknames.Windowing.Base;
using PetRenamer.PetNicknames.Windowing.Enums;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Header;

internal static class ModeToggle
{
    static float cutHeight;
    static float expanedHeight;

    public static void Draw(in PetWindow petWindow)
    {
        Vector2 lastCursorPos = ImGui.GetCursorPos();

        float height = ImGui.GetContentRegionAvail().Y;

        cutHeight = height * 0.65f;
        expanedHeight = height * 1.8f;

        ImGuiStylePtr style = ImGui.GetStyle();

        lastCursorPos += new Vector2(0, (height - cutHeight - style.FramePadding.Y * 2) * 0.5f);

        ImGui.SetCursorPos(ImGui.GetCursorPos() + lastCursorPos);

        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0.5f, 1f, 1f));
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.3f, 1f, 1f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.36f, 0.36f, 1f, 1f));
        DrawFor(in petWindow, PetWindowMode.Minion);
        ImGui.PopStyleColor(3);

        ImGui.SameLine(0, 0);

        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 1f, 0.5f, 1f));
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.8f, 0.3f, 1f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.36f, 1f, 0.36f, 1f));
        DrawFor(in petWindow, PetWindowMode.BattlePet);
        ImGui.PopStyleColor(3);

        ImGui.SetCursorPos(ImGui.GetCursorPos() - lastCursorPos);
    }


    static void DrawFor(in PetWindow petWindow, PetWindowMode mode)
    {
        if (petWindow.CurrentMode == mode)
        {
            ModeToggleNode.DrawDisabled(expanedHeight, cutHeight);
        }
        else
        {
            if (ModeToggleNode.Draw(expanedHeight, cutHeight))
            {
                petWindow.SetPetMode(mode);
            }
        }
    }
}
