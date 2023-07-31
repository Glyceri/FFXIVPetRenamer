using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
public class PetListWindow : PetWindow
{
    SheetUtils sheetUtils { get; set; } = null!;
    StringUtils stringUtils { get; set; } = null!;
    PlayerUtils playerUtils { get; set; } = null!;

    int maxBoxHeight = 730;

    public PetListWindow() : base("Minion List", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        IsOpen = true;

        sheetUtils = PluginLink.Utils.Get<SheetUtils>();
        stringUtils = PluginLink.Utils.Get<StringUtils>();
        playerUtils = PluginLink.Utils.Get<PlayerUtils>();

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(800, 800),
            MaximumSize = new Vector2(800, 10000000)
        };
    }

    public unsafe override void Draw()
    {
        int minionCount = PluginLink.Configuration.users!.Length;
        float calcedSize = minionCount * Styling.ListButton.Y + 10;
        if (calcedSize < maxBoxHeight) calcedSize = maxBoxHeight;
        int counter = 1;

        PlayerData playerData = playerUtils.GetPlayerData()!.Value;
        byte playerGender = playerData.gender;

        ImGui.BeginListBox("##<1>", new System.Numerics.Vector2(780, 32));
        ImGui.Button($"{playerData.playerName}", Styling.ListButton);                                       ImGui.SameLine();
        ImGui.Button($"{sheetUtils.GetWorldName(playerData.homeWorld)}", Styling.ListButton);               ImGui.SameLine();
        ImGui.Button($"{sheetUtils.GetRace(playerData.race, playerData.gender)}", Styling.ListButton);      ImGui.SameLine();
        ImGui.Button($"{sheetUtils.GetGender(playerGender)}", Styling.ListButton);
        ImGui.EndListBox();

        ImGui.BeginListBox("##<2>", new System.Numerics.Vector2(780, calcedSize));
        foreach (SerializableNickname nickname in PluginLink.Configuration.users!)
        {
            string currentPetName = stringUtils.MakeTitleCase(sheetUtils.GetPetName(nickname.ID));

            ImGui.Button(nickname.ID.ToString() + $"##<{counter++}>", Styling.ListIDField);                 ImGui.SameLine();
            ImGui.Button(currentPetName + $"##<{counter++}>", Styling.ListButton);                          ImGui.SameLine();
            if (ImGui.Button($"{nickname.Name} ##<{counter++}>", Styling.ListNameButton)) 
                PluginLink.WindowHandler.GetWindow<MainWindow>().OpenForId(nickname.ID);                    ImGui.SameLine();
            if (ImGui.Button("X" + $"##<{counter++}>", Styling.SmallButton))
               PluginLink.Utils.Get<ConfigurationUtils>().RemoveNickname(nickname.ID);
        }
        ImGui.EndListBox();
    }
}
