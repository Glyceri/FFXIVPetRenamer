using ImGuiNET;

namespace PetRenamer.Windows;

public abstract class InitializablePetWindow : PetWindow
{
    protected InitializablePetWindow(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow) { }

    public abstract void OnInitialized();
}