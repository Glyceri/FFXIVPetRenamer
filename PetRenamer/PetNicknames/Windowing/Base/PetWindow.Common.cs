using PetRenamer.PetNicknames.Windowing.Componenents;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Base;

internal partial class PetWindow
{
    Node? activeModeToggleNode = null;

    protected void EnableModeToggle()
    {
        if (activeModeToggleNode != null) return;
        activeModeToggleNode = new ModeToggleNode(DalamudServices);
        PrepependNode(TopLeftAnchor, activeModeToggleNode);
    }

    protected void DisableModeToggle()
    {
        if (activeModeToggleNode == null) return;
        RemoveNode(TopLeftAnchor, activeModeToggleNode);
        activeModeToggleNode?.Dispose();
        activeModeToggleNode = null;
    }
}
