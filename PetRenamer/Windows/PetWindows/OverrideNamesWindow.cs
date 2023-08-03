using ImGuiNET;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
internal class OverrideNamesWindow : PetWindow
{
    public OverrideNamesWindow() : base("Import Minion Names", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize, false)
    {

    }
}

