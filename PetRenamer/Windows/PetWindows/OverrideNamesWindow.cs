using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System;
using System.Collections.Generic;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
internal class OverrideNamesWindow : PetWindow
{
    SheetUtils sheetUtils { get; set; } = null!;
    StringUtils stringUtils { get; set; } = null!;
    ConfigurationUtils configurationUtils { get; set; } = null!;
    NicknameUtils nicknameUtils { get; set; } = null!;

    SerializableUserV2 importedUser { get; set; } = null!;
    SerializableUserV2 alreadyExistingUser { get; set; } = null!;

    int maxBoxHeight = 670;

    public OverrideNamesWindow() : base("Import Minion Names", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        sheetUtils = PluginLink.Utils.Get<SheetUtils>();
        stringUtils = PluginLink.Utils.Get<StringUtils>();
        configurationUtils = PluginLink.Utils.Get<ConfigurationUtils>();
        nicknameUtils = PluginLink.Utils.Get<NicknameUtils>();

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(800, 800),
            MaximumSize = new Vector2(800, 800)
        };
    }

    public bool SetImportString(string importString)
    {
        importedUser = null!;
        if (!importString.StartsWith("[PetExport]")) return false;
        try
        {
            string[] splitLines = importString.Split('\n');
            if (splitLines.Length <= 2) return false; 
            try
            {
                string userName = splitLines[1];
                ushort homeWorld = ushort.Parse(splitLines[2]);

                List<SerializableNickname> nicknames = new List<SerializableNickname>();
                try
                {
                    for (int i = 3; i < splitLines.Length; i++)
                    {
                        string[] splitNickname = splitLines[i].Split('^');
                        if (splitNickname.Length < 1) continue;
                        if (!int.TryParse(splitNickname[0].Replace("ID:", ""), out int ID)) continue;
                        string nickname = splitNickname[1].Replace("Name:", "");
                        nicknames.Add(new SerializableNickname(ID, nickname));
                    }
                }
                catch (Exception e) { Dalamud.Logging.PluginLog.Log($"Import Error occured [SerializableNickname]: {e}"); }

                importedUser = new SerializableUserV2(nicknames.ToArray(), userName, homeWorld);
            }
            catch (Exception e) { Dalamud.Logging.PluginLog.Log($"Import Error occured [SerializableUser]: {e}"); }
        }
        catch (Exception e) { Dalamud.Logging.PluginLog.Log($"Import Error occured [Overall]: {e}"); return false; }

        return true;
    }

    public unsafe override void OnDraw()
    {
        if (importedUser == null) return;
        alreadyExistingUser = configurationUtils.GetUserV2(importedUser);
        DrawUserHeader();
        DrawList();
        DrawFooter();
    }

    void DrawFooter()
    {
        ImGui.NewLine();
        ImGui.SameLine(638);
        if(Button($"Save Imported List##importListSave", Styling.ListButton))
        {
            if(alreadyExistingUser == null)
            {
                configurationUtils.AddNewUserV2(importedUser);
            }
            else
            {
                alreadyExistingUser.nicknames = importedUser.nicknames;
                PluginLink.Configuration.Save();
            }

            IsOpen = false;
        }
    }

    void DrawUserHeader()
    {
        BeginListBox("##<1>", new System.Numerics.Vector2(780, 32));
        Label($"{importedUser.username}", Styling.ListButton); ImGui.SameLine();
        Label($"{sheetUtils.GetWorldName(importedUser.homeworld)}", Styling.ListButton); ImGui.SameLine(0, 315);
        if (alreadyExistingUser == null) NewLabel("New User", Styling.ListButton);
        else Label("User Status: " +  "Exists", Styling.ListButton);
        ImGui.EndListBox();
        ImGui.NewLine();
    }

    void DrawList()
    {
        int counter = 10;
        BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeight));
        DrawListHeader();

        foreach (SerializableNickname nickname in importedUser.nicknames)
        {
            
            string currentPetName = stringUtils.MakeTitleCase(sheetUtils.GetPetName(nickname.ID));
            if (IsExactSame(nickname)) Label("=", Styling.SmallButton);
            else if (HasNickname(nickname)) OverrideLabel("O", Styling.SmallButton); 
            else NewLabel("+", Styling.SmallButton);
            
            ImGui.SameLine();
            Label(nickname.ID.ToString() + $"##internal<{counter++}>", Styling.ListIDField); ImGui.SameLine();
            Label(currentPetName + $"##internal<{counter++}>", Styling.ListButton); ImGui.SameLine();
            Label($"{nickname.Name} ##internal<{counter++}>", Styling.ListButton);
        }
        ImGui.EndListBox();
    }

    void DrawListHeader()
    {
        Label("", Styling.SmallButton); ImGui.SameLine();
        Label("Minion ID", Styling.ListIDField); ImGui.SameLine();
        Label("Minion Name", Styling.ListButton); ImGui.SameLine();
        Label("Custom Minion name", Styling.ListButton); ImGui.SameLine();
        ImGui.NewLine();
    }

    bool IsExactSame(SerializableNickname nickname)
    {
        if (alreadyExistingUser == null) return false;
        return nicknameUtils.IsSameV2(alreadyExistingUser, nickname.ID, nickname.Name);
    }

    bool HasNickname(SerializableNickname nickname)
    {
        if (alreadyExistingUser == null) return false;
        return nicknameUtils.HasIDV2(alreadyExistingUser, nickname.ID);
    }
}

