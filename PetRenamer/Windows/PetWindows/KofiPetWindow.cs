using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;
using System.Diagnostics;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
public class KofiPetWindow : PetWindow
{
    public KofiPetWindow() : base("Pet Nicknames Kofi", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(495, 119);
        SizeCondition = ImGuiCond.Always;
    }

    public override void OnDraw()
    {
        OverrideLabel("Consider supporting me on Ko-fi. (I will literally use this to buy my dog toys :D)", new Vector2(ContentAvailableX, BarSize));
        if (KofiButton("Ko-Fi", new Vector2(ContentAvailableX, BarSize))) Process.Start(new ProcessStartInfo { FileName = "https://ko-fi.com/glyceri", UseShellExecute = true });
        if (Checkbox("Display Ko-fi button", ref PluginLink.Configuration.showKofiButton)) PluginLink.Configuration.Save();
    }
}
