using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

//[PersistentPetWindow]
public class CreditsWindow : PetWindow
{
    public CreditsWindow() : base("Credits",
   ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
   ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(495, 90);
        SizeCondition = ImGuiCond.Always;
    }

    public override void OnDraw()
    {
        Label("Created by: Glyceri", Styling.ListNameButton);
        Label("In loving memory of: Bruno", Styling.ListNameButton);
    }
}
