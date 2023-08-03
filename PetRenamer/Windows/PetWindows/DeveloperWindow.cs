using ImGuiNET;

namespace PetRenamer.Windows.PetWindows;

internal class DeveloperWindow : PetWindow
{
    public DeveloperWindow() : base("Dev Window Pet Renamer", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        IsOpen = true;
    }
}

