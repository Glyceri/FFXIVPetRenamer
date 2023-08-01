using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
public class CreditsWindow : PetWindow
{
    public CreditsWindow() : base("Credits",
   ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
   ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(300, 75);
        SizeCondition = ImGuiCond.Always;
    }

    public override void OnDraw()
    {
        ImGui.TextColored(new Vector4(0.6f, 1, 1, 1), "Created by: Glyceri");
        ImGui.TextColored(new Vector4(0.6f, 1, 1, 1), "In loving memory of: Bruno");
    }
}
