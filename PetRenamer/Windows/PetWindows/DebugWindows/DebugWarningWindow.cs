using System.Numerics;
using ImGuiNET;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;
#if DEBUG
[PersistentPetWindow]
#endif
public class DebugWarningWindow : PetWindow
{
    public DebugWarningWindow() : base(
        "DEBUG WARNING",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoFocusOnAppearing)
    {
        Size = new Vector2(232, 225);
        SizeCondition = ImGuiCond.Always;
        IsOpen = true;
    }

    public override void OnDraw()
    {
        Label("!WARNING! DEBUG MODE !WARNING!", Styling.ListButton);
    }
}
