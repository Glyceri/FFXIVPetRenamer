using Dalamud.Interface.Windowing;
using ImGuiNET;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Windows;

public abstract class PetWindow : Window, IDisposableRegistryElement
{ 
    protected PetWindow(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow) { }

    public void Dispose() => OnDispose();
    protected virtual void OnDispose() { }
}
