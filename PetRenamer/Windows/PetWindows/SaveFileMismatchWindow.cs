using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
internal class SaveFileMismatchWindow : PetWindow
{
    public SaveFileMismatchWindow() : base("Pet Nicknames Save File Mismatch", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse , true)
    {
        Size = new System.Numerics.Vector2(500, 75);

        if (Configuration.currentSaveFileVersion < PluginLink.Configuration.Version)
            IsOpen = true;
    }

    public override void OnDraw()
    {
        TextColoured(StylingColours.defaultText, $"Your save file version is NEWER than currently supported. [{PluginLink.Configuration.Version}:{Configuration.currentSaveFileVersion}]\nPlease disable the Pet Nicknames plugin and update or risk corrupting your savefile!");
    }
}
