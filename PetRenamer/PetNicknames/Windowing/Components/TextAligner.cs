using ImGuiNET;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components;

internal static class TextAligner
{
    public static void Align(TextAlignment alignment)
    {
        Vector2 basicAlign = new Vector2(0.5f, 0.5f);

        if (alignment == TextAlignment.Left) basicAlign = new Vector2(0, 0.5f);
        if (alignment == TextAlignment.Right) basicAlign = new Vector2(1, 0.5f);

        ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, basicAlign);
    }

    public static void PopAlignment()
    {
        ImGui.PopStyleVar();
    }
}

public enum TextAlignment
{
    Centre,
    Left,
    Right
}
