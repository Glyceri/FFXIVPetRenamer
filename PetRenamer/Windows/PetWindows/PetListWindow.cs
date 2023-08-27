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
using PetRenamer.Core.PettableUserSystem;
using System.Xml.Linq;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
[ModeTogglePetWindow]
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

    PettableUser user = null!;

    public unsafe override void OnDraw()
    {
        if (PluginLink.Configuration.serializableUsersV3!.Length == 0) return;
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        if ((user = PluginLink.PettableUserHandler.LocalUser()!) == null) return;
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
            user.SerializableUser.LoopThrough(nickname =>
            {
                if (nickname.Item1 >= -1) return;
                string currentPetName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.Item1));

                Label(nickname.Item1.ToString() + $"##<{counter++}>", Styling.ListIDField); ImGui.SameLine();
                Label(currentPetName + $"##<{counter++}>", Styling.ListButton); ImGui.SameLine();
                if (Button($"{nickname.Item2} ##<{counter++}>", Styling.ListNameButton))
                    PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForBattleID(nickname.Item1, true); ImGui.SameLine();
                if (XButton("X" + $"##<{counter++}>", Styling.SmallButton))
                {
                    user.SerializableUser.RemoveNickname(nickname.Item1, true);
                    PluginLink.Configuration.Save();
                }
            });
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
        BeginListBox("##<1>", new System.Numerics.Vector2(780, 32));
        Button($"{PluginHandlers.ClientState.LocalPlayer!.Name}", Styling.ListButton); ImGui.SameLine();
        Label($"{SheetUtils.instance.GetWorldName((ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id)}", Styling.ListButton); ImGui.SameLine();
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
                string exportString = string.Concat("[PetExport]\n", user.UserName.ToString(), "\n", user.Homeworld.ToString(), "\n");
                user.SerializableUser.LoopThrough(nickname =>
                {
                    exportString += $"{nickname.Item1}^{nickname.Item2}";
                });
                string convertedString = Convert.ToBase64String(Encoding.Unicode.GetBytes(exportString));
                ImGui.SetClipboardText(convertedString);
            }
            catch (Exception e) { PluginLog.Log($"Export Error occured: {e}"); }
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
            catch (Exception e) { PluginLog.Log($"Import Error occured: {e}"); }
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
            user.SerializableUser.LoopThrough(nickname =>
            {
                if (nickname.Item1 <= 0) return;
                string currentPetName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.Item1));

                Label(nickname.Item1.ToString() + $"##<{counter++}>", Styling.ListIDField); ImGui.SameLine();
                Label(currentPetName + $"##<{counter++}>", Styling.ListButton); ImGui.SameLine();
                if (Button($"{nickname.Item2} ##<{counter++}>", Styling.ListNameButton))
                    PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForId(nickname.Item1, true); ImGui.SameLine();
                if (XButton("X" + $"##<{counter++}>", Styling.SmallButton))
                {
                    user.SerializableUser.RemoveNickname(nickname.Item1, true);
                    PluginLink.Configuration.Save();
                }
            });
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
                PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForId(nickname.ID, true);
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
