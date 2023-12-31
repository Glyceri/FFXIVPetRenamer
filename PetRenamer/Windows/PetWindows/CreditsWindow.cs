using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
public class CreditsWindow : PetWindow
{
    public CreditsWindow() : base("Pet Nicknames Credits", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(495, 90);
        SizeCondition = ImGuiCond.Always;
    }

    public override void OnDraw()
    {
        Label("Created by: Glyceri", new Vector2(ContentAvailableX, BarSize));
        Label("In loving memory of: Bruno", new Vector2(ContentAvailableX, BarSize));
    }
}
