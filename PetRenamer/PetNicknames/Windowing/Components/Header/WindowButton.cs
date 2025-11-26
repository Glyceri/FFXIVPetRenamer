using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using PetRenamer.PetNicknames.Windowing.Base;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Header;

internal static class WindowButton
{
    public static float Width
        => ImGui.GetContentRegionAvail().Y;

    public static void Draw<T>(in WindowHandler handler, in Configuration configuration, FontAwesomeIcon icon, string tooltip) where T : PetWindow
    {
        T? window = handler.GetWindow<T>();

        if (window == null)
        {
            return;
        }

        bool isActive = window.IsOpen;

        ImGui.BeginDisabled(isActive && !configuration.quickButtonsToggle);

        float size = Width;

        ImGui.PushFont(UiBuilder.IconFont);

        // Hehe sex
        TextAligner.Align(TextAlignment.Right);
        bool shouldDoWindow = ImGui.Button($"{icon.ToIconString()}##quickButton_{WindowHandler.InternalCounter}", new Vector2(size, size));
        TextAligner.PopAlignment();

        ImGui.PopFont();

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip(tooltip);
        }

        ImGui.EndDisabled();

        if (!shouldDoWindow)
        {
            return;
        }

        if (configuration.quickButtonsToggle)
        {
            window.Toggle();
        }
        else
        {
            window.Open();
        }
    }
}
