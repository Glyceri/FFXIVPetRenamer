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

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
[ModeTogglePetWindow]
public class PetListWindow : PetWindow
{
    int maxBoxHeight = 675;
    int maxBoxHeightBattle = 631;
    int maxBoxHeightSharing = 590;

    public PetListWindow() : base("Minion List", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(800, 883),
            MaximumSize = new Vector2(800, 883)
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
        ImGui.Indent(220);
        DrawExportHeader();
        ImGui.Unindent(220);
    }

    public override unsafe void OnDrawNormal() => DrawList();
    public override unsafe void OnDrawBattlePet() => DrawBattlePetList();
    public override unsafe void OnDrawSharing() => DrawSharing();

    SerializableUserV3 importedUser { get; set; } = null!;
    SerializableUserV3 alreadyExistingUser { get; set; } = null!;

    unsafe void DrawSharing()
    {
        if (importedUser == null) return;
        foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
        {
            if (user.SerializableUser == null) continue;
            if (user.SerializableUser.username.Trim().ToLower() == importedUser.username.Trim().ToLower() &&
                user.SerializableUser.homeworld == importedUser.homeworld)
            {
                alreadyExistingUser = user.SerializableUser;
                break;
            }
        }
        DrawUserHeaderSharing();
        DrawListSharing();
        DrawFooterSharing();
    }

    void DrawFooterSharing()
    {
        NewLine();
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

                foreach ((int, string) nickname in DeletesNicknames(importedUser))
                {
                    alreadyExistingUser.RemoveNickname(nickname.Item1, false);
                }
                alreadyExistingUser = null!;
                importedUser = null!;
            }
            PluginLink.Configuration.Save();
        }
    }

    void DrawUserHeaderSharing()
    {
        BeginListBox("##<list header>", new System.Numerics.Vector2(780, 32));
        Label($"{StringUtils.instance.MakeTitleCase(importedUser.username)}", Styling.ListButton); SameLine();
        Label($"{SheetUtils.instance.GetWorldName(importedUser.homeworld)}", Styling.ListButton); ImGui.SameLine(0, 315);
        if (alreadyExistingUser == null) NewLabel("New User", Styling.ListButton);
        else Label("User Status: " + "Exists", Styling.ListButton);
        ImGui.EndListBox();
        NewLine();
    }

    void DrawListSharing()
    {
        int counter = 10;
        BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeightSharing));
        DrawListHeaderSharing();

        (int, string)[] deletedNicknames = DeletesNicknames(importedUser);

        foreach ((int, string) nickname in deletedNicknames)
        {
            string currentPetName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.Item1));
            XButtonError("X", Styling.SmallButton);
            SameLine();
            Label(nickname.Item1.ToString() + $"##internal<{counter++}>", Styling.ListIDField); SameLine();
            Label(currentPetName + $"##internal<{counter++}>", Styling.ListButton); SameLine();
            Label($"{nickname.Item2} ##internal<{counter++}>", Styling.ListButton);
        }

        importedUser.LoopThrough(nickname =>
        {
            string currentPetName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.Item1));
            if (IsExactSame(nickname)) Label("=", Styling.SmallButton);
            else if (HasNickname(nickname)) OverrideLabel("O", Styling.SmallButton);
            else NewLabel("+", Styling.SmallButton);

            SameLine();
            Label(nickname.Item1.ToString() + $"##internal<{counter++}>", Styling.ListIDField); SameLine();
            Label(currentPetName + $"##internal<{counter++}>", Styling.ListButton); SameLine();
            Label($"{nickname.Item2} ##internal<{counter++}>", Styling.ListButton);
        });
        ImGui.EndListBox();
    }

    void DrawListHeaderSharing()
    {
        Label("", Styling.SmallButton); SameLine();
        Label("Minion ID", Styling.ListIDField); SameLine();
        Label("Minion Name", Styling.ListButton); SameLine();
        Label("Custom Minion name", Styling.ListButton); SameLine();
        NewLine();
    }

    bool IsExactSame((int, string) nickname)
    {
        if (alreadyExistingUser == null) return false;
        bool exists = false;
        alreadyExistingUser.LoopThroughBreakable(nName =>
        {
            if (nName.Item1 == nickname.Item1 && nName.Item2 == nickname.Item2)
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
                catch (Exception e) { PluginLog.Log($"Import Error occured [SerializableNickname]: {e}"); }

                importedUser = new SerializableUserV3(ids.ToArray(), names.ToArray(), userName, homeWorld);
            }
            catch (Exception e) { PluginLog.Log($"Import Error occured [SerializableUser]: {e}"); }
        }
        catch (Exception e) { PluginLog.Log($"Import Error occured [Overall]: {e}"); return false; }

        return true;
    }

    void DrawBattlePetList()
    {
        DrawBattlePetWarningHeader();
        int counter = 10;
        BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeightBattle));

        if (openedAddPet) DrawOpenedNewPet();
        else
            user.SerializableUser.LoopThrough(nickname =>
            {
                if (nickname.Item1 >= -1) return;
                string currentPetName = StringUtils.instance.MakeTitleCase(RemapUtils.instance.PetIDToName(nickname.Item1));

                Label(nickname.Item1.ToString() + $"##<{counter++}>", Styling.ListIDField); SameLine();
                SetTooltipHovered($"Pet ID: {nickname.Item1}");
                Label(currentPetName + $"##<{counter++}>", Styling.ListButton); SameLine();
                SetTooltipHovered($"Pet Type: {StringUtils.instance.MakeTitleCase(currentPetName)}");
                if (Button($"{nickname.Item2} ##<{counter++}>", Styling.ListNameButton))
                    PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForBattleID(nickname.Item1, true);
                SetTooltipHovered($"Rename: {nickname.Item2}");
                SameLine();
                if (XButton("X" + $"##<{counter++}>", Styling.SmallButton))
                {
                    user.SerializableUser.RemoveNickname(nickname.Item1, true);
                    PluginLink.Configuration.Save();
                }
                SetTooltipHovered($"Clears the nickname!");
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
        Button($"{StringUtils.instance.MakeTitleCase(PluginHandlers.ClientState.LocalPlayer!.Name.ToString())}", Styling.ListButton); SameLine();
        Label($"{SheetUtils.instance.GetWorldName((ushort)PluginHandlers.ClientState.LocalPlayer!.HomeWorld.Id)}", Styling.ListButton); SameLine();
        ImGui.EndListBox();
        NewLine();
    }

    public void DrawExportHeader()
    {
        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        int counter = 30;
        BeginListBox("##<clipboard>", new System.Numerics.Vector2(318, 60));
        Label("A friend can import your code to see your names.", new System.Numerics.Vector2(310, 24));
        if (Button($"Export to Clipboard##clipboardExport{counter++}", Styling.ListButton))
        {
            try
            {
                string exportString = string.Concat("[PetExport]\n", user.UserName.ToString(), "\n", user.Homeworld.ToString(), "\n");
                for(int i = 0; i < user.SerializableUser.length; i++)
                {
                    exportString += $"{user.SerializableUser.ids[i]}^{user.SerializableUser.names[i]}\n";
                }
                string convertedString = Convert.ToBase64String(Encoding.Unicode.GetBytes(exportString));
                ImGui.SetClipboardText(convertedString);
            }
            catch (Exception e) { PluginLog.Log($"Export Error occured: {e}"); }
        }
        SetTooltipHovered("Exports ALL your nicknames to a list.\nYou can send this list to anyone.\nFor example: Paste this text into Discord and let a friend copy it.");
        SameLine();
        if (Button($"Import from Clipboard##clipboardImport{counter++}", Styling.ListButton))
        {
            try
            {
                string gottenText = Encoding.Unicode.GetString(Convert.FromBase64String(ImGui.GetClipboardText()));
                SetPetMode(PetMode.ShareMode);
                PetListWindow window = PluginLink.WindowHandler.GetWindow<PetListWindow>();
                if (window.SetImportString(gottenText))
                    window.IsOpen = true;
            }
            catch (Exception e) { PluginLog.Log($"Import Error occured: {e}"); }
        }
        SetTooltipHovered("After having copied a list of names from a friend.\nClicking this button will result into importing all their nicknames \nallowing you to see them for yourself.");
        ImGui.EndListBox();
        ImGui.NewLine();
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

                Label(nickname.Item1.ToString() + $"##<{counter++}>", Styling.ListIDField);
                SetTooltipHovered($"Minion ID: {nickname.Item1}");
                SameLine();
                Label(currentPetName + $"##<{counter++}>", Styling.ListButton); SameLine();
                SetTooltipHovered($"Minion Name: {StringUtils.instance.MakeTitleCase(currentPetName)}");
                if (Button($"{nickname.Item2} ##<{counter++}>", Styling.ListNameButton))
                    PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForId(nickname.Item1, true); SameLine();
                SetTooltipHovered($"Rename: {nickname.Item2}");
                if (XButton("X" + $"##<{counter++}>", Styling.SmallButton))
                {
                    user.SerializableUser.RemoveNickname(nickname.Item1, true);
                    PluginLink.Configuration.Save();
                }
                SetTooltipHovered($"Deletes the nickname!");
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
        SetTooltipHovered($"Filter on Minion ID or Name.");

        ImGui.SameLine(0, 41);
        if (XButton("X##ForOpenedPet", Styling.SmallButton))
        {
            openedAddPet = false;
            foundNicknames = new List<SerializableNickname>();
        }
        SetTooltipHovered($"Stop looking for a new pet?");
        NewLine();

        foreach (SerializableNickname nickname in foundNicknames)
        {
            Label(nickname.ID.ToString() + $"##<{counter++}>", Styling.ListIDField); SameLine();
            Label(StringUtils.instance.MakeTitleCase(nickname.Name) + $"##<{counter++}>", Styling.ListButton); SameLine();
            if (Button("+" + $"##<{counter++}>", Styling.SmallButton))
            {
                openedAddPet = false;
                PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForId(nickname.ID, true);
            }
            SetTooltipHovered($"Add {StringUtils.instance.MakeTitleCase(nickname.Name)} [{nickname.ID}] to the list?");
        }
    }

    void DrawListHeader()
    {
        Label("Minion ID", Styling.ListIDField); SameLine();
        Label("Minion Name", Styling.ListButton); SameLine();
        Label("Custom Minion name", Styling.ListNameButton); SameLine();
        if (XButton("+", Styling.SmallButton)) openedAddPet = true;

        SetTooltipHovered($"Add a new pet.");

        if (!openedAddPet)
            NewLine();
    }
}
