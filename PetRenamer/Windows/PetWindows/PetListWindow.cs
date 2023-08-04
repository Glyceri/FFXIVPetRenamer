﻿using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;
using System;
using System.Text;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
public class PetListWindow : PetWindow
{
    SheetUtils sheetUtils { get; set; } = null!;
    StringUtils stringUtils { get; set; } = null!;
    PlayerUtils playerUtils { get; set; } = null!;
    ConfigurationUtils configurationUtils { get; set; } = null!;
    NicknameUtils nicknameUtils { get; set; } = null!;

    int maxBoxHeight = 670;

    public PetListWindow() : base("Minion List", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        sheetUtils = PluginLink.Utils.Get<SheetUtils>();
        stringUtils = PluginLink.Utils.Get<StringUtils>();
        playerUtils = PluginLink.Utils.Get<PlayerUtils>();
        configurationUtils = PluginLink.Utils.Get<ConfigurationUtils>();
        nicknameUtils = PluginLink.Utils.Get<NicknameUtils>();

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(800, 800),
            MaximumSize = new Vector2(800, 800)
        };
    }

    public unsafe override void OnDraw()
    {
        if (PluginLink.Configuration.serializableUsers!.Length == 0) return;
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        if (configurationUtils.GetLocalUser() == null) return;

        DrawUserHeader();
        DrawExportHeader();
        DrawList();
    }

    void DrawUserHeader()
    {
        PlayerData? playerData = playerUtils.GetPlayerData();
        if (playerData == null) return;
        byte playerGender = playerData.Value.gender;

        BeginListBox("##<1>", new System.Numerics.Vector2(780, 32));
        Button($"{playerData.Value.playerName}", Styling.ListButton); ImGui.SameLine();
        Label($"{sheetUtils.GetWorldName(playerData.Value.homeWorld)}", Styling.ListButton); ImGui.SameLine();
        Label($"{sheetUtils.GetRace(playerData.Value.race, playerData.Value.gender)}", Styling.ListButton); ImGui.SameLine();
        Label($"{sheetUtils.GetGender(playerGender)}", Styling.ListButton);
        ImGui.EndListBox();
        ImGui.NewLine();
    }

    void DrawExportHeader()
    {
        int counter = 30;
        BeginListBox("##<clipboard>", new System.Numerics.Vector2(780, 32));
        if (Button($"Export to Clipboard##clipboardExport{counter++}", Styling.ListButton))
        {
            try
            {
                SerializableUser localPlayer = configurationUtils.GetLocalUser()!;
                if (localPlayer != null)
                {
                    string exportString = string.Concat("[PetExport]-", localPlayer.username.ToString(), "-", localPlayer.homeworld.ToString(), "-");
                    foreach (SerializableNickname nickname in localPlayer.nicknames)
                        exportString = string.Concat(exportString, nickname.ToString(), "-");
                    string convertedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(exportString));
                    ImGui.SetClipboardText(convertedString);
                }
            }
            catch { }
        }
        ImGui.SameLine();
        if (Button($"Import from Clipboard##clipboardImport{counter++}", Styling.ListButton))
        {
            try
            {
                string gottenText = Encoding.UTF8.GetString(Convert.FromBase64String(ImGui.GetClipboardText())).Replace("-", "\n");
                OverrideNamesWindow window = PluginLink.WindowHandler.GetWindow<OverrideNamesWindow>();
                if(window.SetImportString(gottenText))
                window.IsOpen = true;
            }
            catch { }
        }
        ImGui.EndListBox();
    }

    void DrawList()
    {
        int counter = 10;
        BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeight));
        DrawListHeader();
        if (openedAddPet) DrawOpenedNewPet();
        else
            foreach (SerializableNickname nickname in configurationUtils.GetLocalUser()!.nicknames)
            {
                string currentPetName = stringUtils.MakeTitleCase(sheetUtils.GetPetName(nickname.ID));

                Label(nickname.ID.ToString() + $"##<{counter++}>", Styling.ListIDField); ImGui.SameLine();
                Label(currentPetName + $"##<{counter++}>", Styling.ListButton); ImGui.SameLine();
                if (Button($"{nickname.Name} ##<{counter++}>", Styling.ListNameButton))
                    PluginLink.WindowHandler.GetWindow<MainWindow>().OpenForId(nickname.ID); ImGui.SameLine();
                if (XButton("X" + $"##<{counter++}>", Styling.SmallButton))
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
        if (InputText("Search by minion name or ID", ref minionSearchField, 64, ImGuiInputTextFlags.CallbackEdit))
            foundNicknames = sheetUtils.GetThoseThatContain(minionSearchField);

        ImGui.SameLine(0, 41);
        if (XButton("X##ForOpenedPet", Styling.SmallButton))
        {
            openedAddPet = false;
            minionSearchField = string.Empty;
            foundNicknames = new List<SerializableNickname>();
        }

        ImGui.NewLine();

        foreach (SerializableNickname nickname in foundNicknames)
        {
            Label(nickname.ID.ToString() + $"##<{counter++}>", Styling.ListIDField); ImGui.SameLine();
            Label(stringUtils.MakeTitleCase(nickname.Name) + $"##<{counter++}>", Styling.ListButton); ImGui.SameLine();
            if (Button("+" + $"##<{counter++}>", Styling.SmallButton))
            {
                openedAddPet = false;
                if (!nicknameUtils.ContainsLocal(nickname.ID)) configurationUtils.SetLocalNickname(nickname.ID, string.Empty);
                PluginLink.WindowHandler.GetWindow<MainWindow>().OpenForId(nickname.ID);
            }
        }
    }

    void DrawListHeader()
    {
        Label("Minion ID", Styling.ListIDField); ImGui.SameLine();
        Label("Minion Name", Styling.ListButton); ImGui.SameLine();
        Label("Custom Minion name", Styling.ListNameButton); ImGui.SameLine();
        if (XButton("+", Styling.SmallButton)) openedAddPet = true;
        if (!openedAddPet)
            ImGui.NewLine();
    }
}
