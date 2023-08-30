using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.Translations;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
public class CreditsWindow : PetWindow
{
    public CreditsWindow() : base(Translate.GetValue("Credits"),
   ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
   ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(495, 90);
        SizeCondition = ImGuiCond.Always;
    }

    public override void OnDraw()
    {
        Label($"{Translate.GetValue("Created_By")} Glyceri", Styling.ListNameButton);
        Label($"{Translate.GetValue("In_Memory_Of")} Bruno", Styling.ListNameButton);
    }
}
