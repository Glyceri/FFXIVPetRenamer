using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.AutoRegistry.Interfaces;

namespace PetRenamer.Windows;

public abstract class PetWindow : Window, IDisposableRegistryElement
{ 
    protected PetWindow(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow) { }

    public void Dispose() => OnDispose();
    protected virtual void OnDispose() { }

    protected static class Styling
    {
        public static Vector2 ListButton = new Vector2(150, 25);
        public static Vector2 ListNameButton = new Vector2(480, 25);
        public static Vector2 ListIDField = new Vector2(75, 25);
        public static Vector2 SmallButton = new Vector2(25, 25);
    }
}
