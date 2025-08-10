using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Header;

internal static class ModeToggleNode
{
    public static bool Draw(float expanedHeight ,float cutHeight)
    {
       return ImGui.Button($"##ModeToggle_{WindowHandler.InternalCounter}", new Vector2(expanedHeight, cutHeight));
    }

    public static bool DrawDisabled(float expanedHeight, float cutHeight)
    {
        ImGui.BeginDisabled(true);
        bool value = Draw(expanedHeight, cutHeight);
        ImGui.EndDisabled();
        return value;
    }
}
