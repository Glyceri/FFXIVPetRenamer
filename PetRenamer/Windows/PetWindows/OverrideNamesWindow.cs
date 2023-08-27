using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System;
using System.Collections.Generic;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
internal class OverrideNamesWindow : PetWindow
{
    SerializableUserV3 importedUser { get; set; } = null!;
    SerializableUserV3 alreadyExistingUser { get; set; } = null!;

    int maxBoxHeight = 670;

    public OverrideNamesWindow() : base("Import Minion Names", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
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

                List<int> ids = new List<int>();
                List<string> names = new List<string>();    
                try
                {
                    for (int i = 3; i < splitLines.Length; i++)
                    {
                        string[] splitNickname = splitLines[i].Split('^');
                        if (splitNickname.Length < 1) continue;
                        if (!int.TryParse(splitNickname[0].Replace("ID:", ""), out int ID)) { continue; }
                        string nickname = splitNickname[1].Replace("Name:", "");
                        ids.Add(ID);
                        names.Add(nickname);
                    }
                }
                catch (Exception e) { Dalamud.Logging.PluginLog.Log($"Import Error occured [SerializableNickname]: {e}"); }

                importedUser = new SerializableUserV3(ids.ToArray(), names.ToArray(), userName, homeWorld);
            }
            catch (Exception e) { Dalamud.Logging.PluginLog.Log($"Import Error occured [SerializableUser]: {e}"); }
        }
        catch (Exception e) { Dalamud.Logging.PluginLog.Log($"Import Error occured [Overall]: {e}"); return false; }

        return true;
    }

    public unsafe override void OnDraw()
    {
        if (importedUser == null) return;
        foreach(PettableUser user in PluginLink.PettableUserHandler.Users)
        {
            if (user.SerializableUser == null) continue;
            if (user.SerializableUser.username.Trim().ToLower() == importedUser.username.Trim().ToLower() &&
                user.SerializableUser.homeworld == importedUser.homeworld)
            {
                alreadyExistingUser = user.SerializableUser;
                break;
            }
        }
        DrawUserHeader();
        DrawList();
        DrawFooter();
    }

    void DrawFooter()
    {
        ImGui.NewLine();
        ImGui.SameLine(638);
        if (Button($"Save Imported List##importListSave", Styling.ListButton))
        {
            if (alreadyExistingUser == null)
            {
                PluginLink.PettableUserHandler.DeclareUser(importedUser, Core.PettableUserSystem.Enums.UserDeclareType.Add);
            }
            else
            {
                importedUser.LoopThrough(nickname =>
                {
                    alreadyExistingUser.SaveNickname(nickname.Item1, nickname.Item2, true, false);
                });

                foreach((int, string) nickname in DeletesNicknames(importedUser))
                {
                    alreadyExistingUser.RemoveNickname(nickname.Item1, false);
                }
                PluginLink.Configuration.Save();
            }
            IsOpen = false;
        }
    }

    void DrawUserHeader()
    {
        BeginListBox("##<1>", new System.Numerics.Vector2(780, 32));
        Label($"{StringUtils.instance.MakeTitleCase(importedUser.username)}", Styling.ListButton); ImGui.SameLine();
        Label($"{SheetUtils.instance.GetWorldName(importedUser.homeworld)}", Styling.ListButton); ImGui.SameLine(0, 315);
        if (alreadyExistingUser == null) NewLabel("New User", Styling.ListButton);
        else Label("User Status: " + "Exists", Styling.ListButton);
        ImGui.EndListBox();
        ImGui.NewLine();
    }

    void DrawList()
    {
        int counter = 10;
        BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeight));
        DrawListHeader();

        (int, string)[] deletedNicknames = DeletesNicknames(importedUser);

        foreach ((int, string) nickname in deletedNicknames)
        {
            string currentPetName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.Item1));
            XButtonError("X", Styling.SmallButton);
            ImGui.SameLine();
            Label(nickname.Item1.ToString() + $"##internal<{counter++}>", Styling.ListIDField); ImGui.SameLine();
            Label(currentPetName + $"##internal<{counter++}>", Styling.ListButton); ImGui.SameLine();
            Label($"{nickname.Item2} ##internal<{counter++}>", Styling.ListButton);
        }

        importedUser.LoopThrough(nickname =>
        {
            string currentPetName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.Item1));
            if (IsExactSame(nickname)) Label("=", Styling.SmallButton);
            else if (HasNickname(nickname)) OverrideLabel("O", Styling.SmallButton);
            else NewLabel("+", Styling.SmallButton);

            ImGui.SameLine();
            Label(nickname.Item1.ToString() + $"##internal<{counter++}>", Styling.ListIDField); ImGui.SameLine();
            Label(currentPetName + $"##internal<{counter++}>", Styling.ListButton); ImGui.SameLine();
            Label($"{nickname.Item2} ##internal<{counter++}>", Styling.ListButton);
        });
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

    bool IsExactSame((int, string) nickname)
    {
        if (alreadyExistingUser == null) return false;
        bool exists = false;
        alreadyExistingUser.LoopThroughBreakable(nName =>
        {
            if(nName.Item1 == nickname.Item1 && nName.Item2 == nickname.Item2)
            {
                exists = true;
                return true;
            }
            return false;
        });
        return exists;
    }

    bool HasNickname((int, string) nickname)
    {
        if (alreadyExistingUser == null) return false;
        bool exists = false;
        alreadyExistingUser.LoopThroughBreakable(nName =>
        {
            if (nName.Item1 == nickname.Item1)
            {
                exists = true;
                return true;
            }
            return false;
        });
        return exists;
    }

    bool IsDeleted(SerializableNickname[] nicknames, SerializableNickname nickname)
    {
        foreach (SerializableNickname nick in nicknames)
            if (nick.ID == nickname.ID)
                return true;

        return false;
    }

    (int, string)[] DeletesNicknames(SerializableUserV3 importedUser)
    {
        List<(int, string)> nicknames = new List<(int, string)>();
        if (alreadyExistingUser == null) return nicknames.ToArray();

        alreadyExistingUser.LoopThrough(nickname => 
        {
            bool exists = false;
            importedUser.LoopThroughBreakable(nickname2 =>
            {
                if (nickname2.Item1 == nickname.Item1)
                {
                    exists = true;
                    return true;
                }
                return false;
            });
            if (exists) return;
            nicknames.Add(nickname);
        });

        return nicknames.ToArray();
    }
}

