using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
public class PetListWindow : InitializablePetWindow
{
    SheetUtils sheetUtils { get; set; } = null!;
    StringUtils stringUtils { get; set; } = null!;

    public PetListWindow() : base("Minion List")
    {
        IsOpen = true;

        sheetUtils = PluginLink.Utils.Get<SheetUtils>();
        stringUtils = PluginLink.Utils.Get<StringUtils>();

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(800, 800),
            MaximumSize = new Vector2(800, 10000000)
        };
    }

    public override void Draw()
    {
        int minionCount = PluginLink.Configuration.users!.Length;
        float calcedSize = minionCount * Styling.ListButton.Y + 10;
        if (calcedSize < 760) calcedSize = 760;
        int counter = 0;

        foreach (SerializableNickname nickname in PluginLink.Configuration.users!)
        {
            string currentPetName = stringUtils.MakeTitleCase(sheetUtils.GetPetName(nickname.ID));

            ImGui.Button(nickname.ID.ToString() + $"##<{counter++}>", Styling.ListIDField);
            ImGui.SameLine();
            ImGui.Button(currentPetName + $"##<{counter++}>", Styling.ListButton);
            ImGui.SameLine();
            if (ImGui.Button(nickname.Name + $"##<{counter++}>", Styling.ListNameButton)) PluginLink.WindowHandler.GetWindow<MainWindow>().OpenForId(nickname.ID);
            ImGui.SameLine();
            if (ImGui.Button("X" + $"##<{counter++}>", Styling.SmallButton))
               PluginLink.Utils.Get<ConfigurationUtils>().RemoveNickname(nickname.ID);
        }
    }

    public override void OnInitialized()
    {

    }
}
