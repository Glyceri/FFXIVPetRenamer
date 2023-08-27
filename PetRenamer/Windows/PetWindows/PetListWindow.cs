using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;
using System;
using System.Text;
using Dalamud.Logging;
using PetRenamer.Core;

namespace PetRenamer.Windows.PetWindows;

//[PersistentPetWindow]
//[ModeTogglePetWindow]
public class PetListWindow : PetWindow
{
    int maxBoxHeight = 670;

    public PetListWindow() : base("Minion List", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(800, 815),
            MaximumSize = new Vector2(800, 815)
        };
    }

    public unsafe override void OnDraw()
    {
        if (PluginLink.Configuration.serializableUsersV2!.Length == 0) return;
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        if (ConfigurationUtils.instance.GetLocalUserV2() == null) return;
        if (petMode == PetMode.BattlePet) openedAddPet = false;
        
        DrawUserHeader();
        DrawExportHeader();
    }

    public override unsafe void OnDrawNormal() => DrawList();
    public override unsafe void OnDrawBattlePet() => DrawBattlePetList();

    void DrawBattlePetList()
    {
        DrawBattlePetWarningHeader();
        int counter = 10;
        BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeight));

        if (openedAddPet) DrawOpenedNewPet();
        else
            foreach (SerializableNickname nickname in ConfigurationUtils.instance.GetLocalUserV2()!.nicknames)
            {
                if (nickname.ID >= -1) continue;
                string currentPetName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.ID));

                Label(nickname.ID.ToString() + $"##<{counter++}>", Styling.ListIDField); ImGui.SameLine();
                Label(RemapUtils.instance.PetIDToName(nickname.ID).ToString() + $"##<{counter++}>", Styling.ListButton); ImGui.SameLine();
                if (Button($"{nickname.Name} ##<{counter++}>", Styling.ListNameButton))
                    PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForBattleID(nickname.ID); ImGui.SameLine();
                if (XButton("X" + $"##<{counter++}>", Styling.SmallButton))
                    ConfigurationUtils.instance.SetLocalNicknameV2(nickname.ID, string.Empty);
            }
        ImGui.EndListBox();
    }

    void DrawBattlePetWarningHeader()
    {
        BeginListBox("##WarningHeader", new System.Numerics.Vector2(780, 40));
        ImGui.TextColored(StylingColours.highlightText, "Please note: If you use /petglamour and change a pets glamour, it will retain the same name.");
        ImGui.EndListBox();
    }

    void DrawUserHeader()
    {
        PlayerData? playerData = PlayerUtils.instance.GetPlayerData();
        if (playerData == null) return;

        BeginListBox("##<1>", new System.Numerics.Vector2(780, 32));
        Button($"{playerData.Value.playerName}", Styling.ListButton); ImGui.SameLine();
        Label($"{SheetUtils.instance.GetWorldName(playerData.Value.homeWorld)}", Styling.ListButton); ImGui.SameLine();
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
                SerializableUserV2 localPlayer = ConfigurationUtils.instance.GetLocalUserV2()!;
                if (localPlayer != null)
                {
                    string exportString = string.Concat("[PetExport]\n", localPlayer.username.ToString(), "\n", localPlayer.homeworld.ToString(), "\n");
                    foreach (SerializableNickname nickname in localPlayer.nicknames)
                        exportString = string.Concat(exportString, nickname.ToSaveString(), "\n");
                    string convertedString = Convert.ToBase64String(Encoding.Unicode.GetBytes(exportString));
                    ImGui.SetClipboardText(convertedString);
                }
            }
            catch (Exception e) { Dalamud.Logging.PluginLog.Log($"Export Error occured: {e}"); }
        }
        ImGui.SameLine();
        if (Button($"Import from Clipboard##clipboardImport{counter++}", Styling.ListButton))
        {
            try
            {
                string gottenText = Encoding.Unicode.GetString(Convert.FromBase64String(ImGui.GetClipboardText()));
                OverrideNamesWindow window = PluginLink.WindowHandler.GetWindow<OverrideNamesWindow>();
                if (window.SetImportString(gottenText))
                    window.IsOpen = true;
            }
            catch (Exception e) { Dalamud.Logging.PluginLog.Log($"Import Error occured: {e}"); }
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
        {
            SerializableUserV2 user = ConfigurationUtils.instance.GetLocalUserV2()!;
            if (user != null)
            {
                foreach (SerializableNickname nickname in user!.nicknames)
                {
                    if (nickname == null) continue;
                    if (nickname.ID <= 0) continue;
                    string currentPetName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.ID));

                    Label(nickname.ID.ToString() + $"##<{counter++}>", Styling.ListIDField); ImGui.SameLine();
                    Label(currentPetName + $"##<{counter++}>", Styling.ListButton); ImGui.SameLine();
                    if (Button($"{nickname.Name} ##<{counter++}>", Styling.ListNameButton))
                        PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForId(nickname.ID); ImGui.SameLine();
                    if (XButton("X" + $"##<{counter++}>", Styling.SmallButton))
                        ConfigurationUtils.instance.RemoveLocalNicknameV2(nickname.ID);
                }
            }
        }
        ImGui.EndListBox();
    }

    bool openedAddPet = false;
    string minionSearchField = string.Empty;
    List<SerializableNickname> foundNicknames = new List<SerializableNickname>();
    void DrawOpenedNewPet()
    {
        int counter = 0;
        string searchField;
        if (InputText("Search by minion name or ID", ref minionSearchField, PluginConstants.ffxivNameSize, ImGuiInputTextFlags.CallbackCompletion))
        {
            searchField = minionSearchField;
            foundNicknames = SheetUtils.instance.GetThoseThatContain(searchField);
        }

        ImGui.SameLine(0, 41);
        if (XButton("X##ForOpenedPet", Styling.SmallButton))
        {
            openedAddPet = false;
            foundNicknames = new List<SerializableNickname>();
        }

        ImGui.NewLine();

        foreach (SerializableNickname nickname in foundNicknames)
        {
            Label(nickname.ID.ToString() + $"##<{counter++}>", Styling.ListIDField); ImGui.SameLine();
            Label(StringUtils.instance.MakeTitleCase(nickname.Name) + $"##<{counter++}>", Styling.ListButton); ImGui.SameLine();
            if (Button("+" + $"##<{counter++}>", Styling.SmallButton))
            {
                openedAddPet = false;
                if (!NicknameUtils.instance.ContainsLocalV2(nickname.ID))
                    ConfigurationUtils.instance.SetLocalNicknameV2(nickname.ID, string.Empty);
                PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForId(nickname.ID);
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
