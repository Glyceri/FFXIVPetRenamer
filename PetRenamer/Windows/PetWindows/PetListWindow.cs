using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
public class PetListWindow : PetWindow
{
    SheetUtils sheetUtils { get; set; } = null!;
    StringUtils stringUtils { get; set; } = null!;
    PlayerUtils playerUtils { get; set; } = null!;
    ConfigurationUtils configurationUtils { get; set; } = null!;
    NicknameUtils nicknameUtils { get; set; } = null!;

    int maxBoxHeight = 725;

    public PetListWindow() : base("Minion List", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        sheetUtils = PluginLink.Utils.Get<SheetUtils>();
        stringUtils = PluginLink.Utils.Get<StringUtils>();
        playerUtils = PluginLink.Utils.Get<PlayerUtils>();
        configurationUtils = PluginLink.Utils.Get<ConfigurationUtils>();
        nicknameUtils = PluginLink.Utils.Get<NicknameUtils>();

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(800, 800),
            MaximumSize = new Vector2(800, 10000000)
        };
    }

    public unsafe override void Draw()
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return;
        
        DrawUserHeader();
        DrawList();
    }

    void DrawUserHeader()
    {
        PlayerData playerData = playerUtils.GetPlayerData()!.Value;
        byte playerGender = playerData.gender;
        
        ImGui.BeginListBox("##<1>", new System.Numerics.Vector2(780, 32));
        ImGui.Button($"{playerData.playerName}", Styling.ListButton); ImGui.SameLine();
        ImGui.Button($"{sheetUtils.GetWorldName(playerData.homeWorld)}", Styling.ListButton); ImGui.SameLine();
        ImGui.Button($"{sheetUtils.GetRace(playerData.race, playerData.gender)}", Styling.ListButton); ImGui.SameLine();
        ImGui.Button($"{sheetUtils.GetGender(playerGender)}", Styling.ListButton);
        ImGui.EndListBox();
    }

    void DrawList()
    {
        int counter = 10;
        ImGui.BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeight));
        DrawListHeader();
        if (openedAddPet) DrawOpenedNewPet();
        else 
        foreach (SerializableNickname nickname in PluginLink.Configuration.serializableUsers![0].nicknames)
        {
            string currentPetName = stringUtils.MakeTitleCase(sheetUtils.GetPetName(nickname.ID));

            ImGui.Button(nickname.ID.ToString() + $"##<{counter++}>", Styling.ListIDField); ImGui.SameLine();
            ImGui.Button(currentPetName + $"##<{counter++}>", Styling.ListButton); ImGui.SameLine();
            if (ImGui.Button($"{nickname.Name} ##<{counter++}>", Styling.ListNameButton))
                PluginLink.WindowHandler.GetWindow<MainWindow>().OpenForId(nickname.ID); ImGui.SameLine();
            if (ImGui.Button("X" + $"##<{counter++}>", Styling.SmallButton))
                PluginLink.Utils.Get<ConfigurationUtils>().RemoveLocalNickname(nickname.ID);
        }
        ImGui.EndListBox();
        
    }

    bool openedAddPet = false;
    string minionSearchField = string.Empty;
    List<SerializableNickname> foundNicknames = new List<SerializableNickname>();
    void DrawOpenedNewPet()
    {
        int counter = 0;
        if(ImGui.InputText("Search by minion name or ID", ref minionSearchField, 64, ImGuiInputTextFlags.CallbackEdit))
            foundNicknames = sheetUtils.GetThoseThatContain(minionSearchField);
        
        ImGui.SameLine();
        if(ImGui.Button("X##ForOpenedPet", Styling.SmallButton))
        {
            openedAddPet = false;
            minionSearchField = string.Empty;
            foundNicknames = new List<SerializableNickname>();
        }

        ImGui.NewLine();

        foreach(SerializableNickname nickname in foundNicknames)
        {
            ImGui.Button(nickname.ID.ToString() + $"##<{counter++}>", Styling.ListIDField); ImGui.SameLine();
            ImGui.Button(stringUtils.MakeTitleCase(nickname.Name) + $"##<{counter++}>", Styling.ListButton); ImGui.SameLine();
            if (ImGui.Button("+" + $"##<{counter++}>", Styling.SmallButton))
            {
                openedAddPet = false;
                if(!nicknameUtils.ContainsLocal(nickname.ID)) configurationUtils.SetLocalNickname(nickname.ID, string.Empty);
                PluginLink.WindowHandler.GetWindow<MainWindow>().OpenForId(nickname.ID);
            }
        }
    }

    void DrawListHeader()
    {
        ImGui.Button("Minion ID", Styling.ListIDField); ImGui.SameLine();
        ImGui.Button("Minion Name", Styling.ListButton); ImGui.SameLine();
        ImGui.Button("Custom Minion name", Styling.ListNameButton); ImGui.SameLine();
        if (ImGui.Button("+", Styling.SmallButton)) openedAddPet = true;
        if(!openedAddPet)
        ImGui.NewLine();
    }
}
