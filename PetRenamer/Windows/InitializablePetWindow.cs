using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;

namespace PetRenamer.Windows;

public abstract class InitializablePetWindow : PetWindow
{
    protected InitializablePetWindow(string name, ImGuiWindowFlags flags = ImGuiWindowFlags.None, bool forceMainWindow = false) : base(name, flags, forceMainWindow) { }

    public abstract void OnInitialized();

    protected static class Styling
    {
        public static Vector2 ListButton = new Vector2(150, 25);
        public static Vector2 ListNameButton = new Vector2(490, 25);
        public static Vector2 ListIDField = new Vector2(75, 25);
        public static Vector2 SmallButton = new Vector2(25, 25);
    }
}