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
using PetRenamer.Core.Ipc.PenumbraIPCHelper;
using ImGuiScene;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Metrics;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
[ModeTogglePetWindow]
public class PetListWindow : PetWindow
{
    int maxBoxHeight = 675;
    int maxBoxHeightBattle = 631;
    int maxBoxHeightSharing = 590;

    public PetListWindow() : base("Pet Nicknames List", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(800, 883),
            MaximumSize = new Vector2(800, 883)
        };

        IsOpen = true;
        SetUserMode(true);
    }

    PettableUser user = null!;
    PettableUser lastUser = null!;
    bool userMode = false;
    bool currentIsLocalUser = false;

    bool _openedAddPet = false;
    bool youSureMode = false;
    PettableUser youSureUser = null!;
    string minionSearchField = string.Empty;
    List<SerializableNickname> foundNicknames = new List<SerializableNickname>();
    bool advancedMode = false;

    int texturePointer = 0;
    string searchField = string.Empty;

    public override void OnDraw()
    {
        if (PluginLink.Configuration.serializableUsersV3!.Length == 0) return;
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        PettableUser localUser = PluginLink.PettableUserHandler.LocalUser()!;
        if ((user ??= localUser!) == null) return;
        if (petMode != PetMode.Normal) SetOpenedAddPet(false);
        if (petMode != PetMode.ShareMode) SetAdvancedMode(false);
        if (petMode == PetMode.ShareMode)
        {
            SetUserMode(false);
            user = localUser!;
        }
        if (user == null) return;
        currentIsLocalUser = user == localUser;
        if (!currentIsLocalUser) SetOpenedAddPet(false);

        DrawUserHeader();
        ImGui.Indent(220);
        DrawExportHeader();
        ImGui.Unindent(220);

        if (user.CompanionChanged || user != lastUser)
            FillTextureList();
    }

    public override unsafe void OnDrawNormal() => HandleOnDrawNormal();
    public override unsafe void OnDrawBattlePet() => HandleOnDrawBattlePetList();
    public override unsafe void OnDrawSharing() => HandleOnDrawSharing();

    internal override void OnPetModeChange(PetMode mode)
    {
        FillTextureList();
    }

    void HandleOnDrawNormal()
    {
        if (user == null) return;
        if (userMode) DrawUserSelect();
        else DrawList();
    }

    void HandleOnDrawBattlePetList()
    {
        if (user == null) return;
        if (userMode) DrawUserSelect();
        else DrawBattlePetList();
    }

    void HandleOnDrawSharing()
    {
        if (user == null) return;
        if (userMode) DrawUserSelect();
        else DrawSharing();
    }

    void DrawUserSelect()
    {
        BeginListBox("##<4>", new System.Numerics.Vector2(780, maxBoxHeightBattle));
        bool buttonPressed = false;
        int counter = 1000;
        texturePointer = 0;

        BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeightBattle));
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 01f));
        ImGui.BeginTable("##Image Table 4", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY);
        DrawFillerBar(2);

        foreach (PettableUser u in PluginLink.PettableUserHandler.Users)
        {
            if (u == null) continue;
            SetColumn(0);
            DrawTexture(PluginLink.PettableUserHandler.GetTexture(u));
            SetColumn(1);

            if (u.LocalUser)
            {
                DrawAdvancedBarWithQuit("Username", u.UserName, ref counter, () =>
                {
                    SetUserMode(false);
                    user = u;
                    buttonPressed = true;
                });
            }
            else
            {
                DrawAdvancedBarWithQuit("Username", u.UserName, ref counter, () =>
                {
                    SetUserMode(false);
                    user = u;
                }, 
                "X", "Remove User", () => 
                {
                    youSureMode = !youSureMode;
                    youSureUser = youSureMode ? u : null!;
                    buttonPressed = true;
                });
            }
            SetTooltipHovered($"Show names from: {u.UserName}");
            if (youSureMode && youSureUser == u)
            {
                TransparentLabel("", new Vector2(1,25));
                DrawYesNoBar($"Are you sure you want to remove: {youSureUser.UserName}@{SheetUtils.instance.GetWorldName(youSureUser.Homeworld)}", 
                    ref counter, 
                    () => 
                    {
                        youSureMode = false;
                        youSureUser = null!;
                        buttonPressed = true;
                        PluginLink.PettableUserHandler.DeclareUser(u.SerializableUser, Core.PettableUserSystem.Enums.UserDeclareType.Remove);
                        PluginLink.Configuration.Save();
                    }, () =>
                    {
                        youSureMode = false;
                        youSureUser = null!;
                        buttonPressed = true;
                    });
            }
            else
            {
                DrawBasicBar("Homeworld", $"{SheetUtils.instance.GetWorldName(u.Homeworld)}", ref counter);
                DrawBasicBar("Petcount", $"Total: {u.SerializableUser.AccurateTotalPetCount()}, Minions: {u.SerializableUser.AccurateMinionCount()}, Battle Pets: {u.SerializableUser.AccurateBattlePetCount()}", ref counter);
            }
            DrawRightHeading(2);
            DrawFillerBar(2);

            if (buttonPressed) break;
        }

        ImGui.EndTable();
        ImGui.PopStyleVar();

        ImGui.EndListBox();

    }

    SerializableUserV3 importedUser { get; set; } = null!;
    SerializableUserV3 alreadyExistingUser { get; set; } = null!;

    unsafe void DrawSharing()
    {
        if (user == null) return;
        if (advancedMode)
        {
            DrawAdvancedSharing();
            return;
        }

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

    void DrawAdvancedSharing()
    {
        DrawAdvancedHeader();
        DrawAdvancedList();
    }

    List<bool> contains = new List<bool>();
    bool current = true;
    void DrawAdvancedList()
    {
        if (contains.Count == 0 && user.SerializableUser.length != 0)
        {
            FillList();
            current = true;
        }

        int counter = 10000;
        BeginListBox("##<6>", new System.Numerics.Vector2(780, maxBoxHeightSharing));


        if (Checkbox($"##{counter++}", ref current))
            for (int i = 0; i < contains.Count; i++)
                contains[i] = current;
        SetTooltipHovered("Toggle all");

        SameLine();
        Label("ID", Styling.ListIDField); SameLine();
        Label("Name", Styling.ListButton); SameLine();
        Label("Custom Name", Styling.ListButton); SameLine();
        NewLine();
        NewLine();

        for (int i = 0; i < user.SerializableUser.length; i++)
        {
            current = contains[i];
            Checkbox($"##{counter++}", ref current);
            SetTooltipHovered("Include pet in export");
            contains[i] = current;
            SameLine();
            Label($"{user.SerializableUser.ids[i]}##{counter++}", Styling.ListIDField); SameLine();
            string basePetName;
            if (user.SerializableUser.ids[i] < -1) basePetName = RemapUtils.instance.PetIDToName(user.SerializableUser.ids[i]);
            else basePetName = SheetUtils.instance.GetPetName(user.SerializableUser.ids[i]);
            Label($"{basePetName}##{counter++}", Styling.ListButton); SameLine();
            Label($"{user.SerializableUser.names[i]}##{counter++}", Styling.ListButton); SameLine();
            NewLine();
        }

        ImGui.EndListBox();

        NewLine();
        ImGui.SameLine(638);

        if (Button($"Export Nicknames List##EmportListSave", Styling.ListButton))
        {
            Export();
            SetAdvancedMode(false);
        }
    }

    void FillList()
    {
        contains.Clear();
        for (int i = 0; i < user.SerializableUser.length; i++)
            contains.Add(true);
    }

    void Export()
    {
        try
        {
            string exportString = string.Concat("[PetExport]\n", user.UserName.ToString(), "\n", user.Homeworld.ToString(), "\n");
            for (int i = 0; i < user.SerializableUser.length; i++)
            {
                if (user.SerializableUser.names[i] != string.Empty && contains[i])
                    exportString += $"{user.SerializableUser.ids[i]}^{user.SerializableUser.names[i]}\n";
            }
            string convertedString = Convert.ToBase64String(Encoding.Unicode.GetBytes(exportString));
            ImGui.SetClipboardText(convertedString);
            exportTimer = 2;
        }
        catch (Exception e) { PluginLog.Log($"Export Error occured: {e}"); errorTimer = 2; }
    }

    void DrawAdvancedHeader()
    {
        BeginListBox("##<list header>", new System.Numerics.Vector2(780, 32));
        Label($"{StringUtils.instance.MakeTitleCase(user.UserName)}", Styling.ListButton); SameLine();
        SetTooltipHovered($"{StringUtils.instance.MakeTitleCase(user.UserName)}");
        Label($"{SheetUtils.instance.GetWorldName(user.Homeworld)}", Styling.ListButton); ImGui.SameLine(0, 315);
        SetTooltipHovered($"{SheetUtils.instance.GetWorldName(user.Homeworld)}");
        ImGui.EndListBox();
        NewLine();
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
            }

            PettableUser curUser = null!;
            foreach (PettableUser user in PluginLink.PettableUserHandler.Users)
                if (user.SerializableUser.Equals(importedUser.username, importedUser.homeworld))
                {
                    curUser = user;
                    break;
                }

            if (curUser != null)
            {
                PenumbraIPCProvider.RedrawBattlePetByIndex(curUser.BattlePetIndex);
                PenumbraIPCProvider.RedrawMinionByIndex(curUser.MinionIndex);
            }

            alreadyExistingUser = null!;
            importedUser = null!;

            PluginLink.Configuration.Save();
        }
    }

    void DrawUserHeaderSharing()
    {
        BeginListBox("##<list header>", new System.Numerics.Vector2(780, 32));
        Label($"{StringUtils.instance.MakeTitleCase(importedUser.username)}", Styling.ListButton); SameLine();
        SetTooltipHovered($"{StringUtils.instance.MakeTitleCase(importedUser.username)}");
        Label($"{SheetUtils.instance.GetWorldName(importedUser.homeworld)}", Styling.ListButton); ImGui.SameLine(0, 315);
        SetTooltipHovered($"{SheetUtils.instance.GetWorldName(importedUser.homeworld)}");
        if (alreadyExistingUser == null) NewLabel("New User", Styling.ListButton);
        else Label("User Status: " + "Exists", Styling.ListButton);
        SetTooltipHovered($"Status of the user");
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
            string currentPetName =
            nickname.Item1 >= 0 ?
            StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.Item1)) :
            StringUtils.instance.MakeTitleCase(RemapUtils.instance.PetIDToName(nickname.Item1));

            XButtonError("X", Styling.SmallButton);
            SetTooltipHovered("Pet name gets removed");
            SameLine();
            Label(nickname.Item1.ToString() + $"##internal<{counter++}>", Styling.ListIDField); SameLine();
            SetTooltipHovered(nickname.Item1.ToString());
            Label(currentPetName + $"##internal<{counter++}>", Styling.ListButton); SameLine();
            SetTooltipHovered(currentPetName);
            Label($"{nickname.Item2} ##internal<{counter++}>", Styling.ListButton);
            SetTooltipHovered(nickname.Item2);
        }

        importedUser.LoopThrough(nickname =>
        {
            string currentPetName =
            nickname.Item1 >= 0 ?
            StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.Item1)) :
            StringUtils.instance.MakeTitleCase(RemapUtils.instance.PetIDToName(nickname.Item1));
            if (IsExactSame(nickname))
            {
                Label("=", Styling.SmallButton);
                SetTooltipHovered("Pet name stays the exact same");
            }
            else if (HasNickname(nickname))
            {
                OverrideLabel("O", Styling.SmallButton);
                SetTooltipHovered("Pet name gets changed");
            }
            else
            {
                NewLabel("+", Styling.SmallButton);
                SetTooltipHovered("Pet name is new");
            }

            SameLine();
            Label(nickname.Item1.ToString() + $"##internal<{counter++}>", Styling.ListIDField); SameLine();
            SetTooltipHovered(nickname.Item1.ToString());
            Label(currentPetName + $"##internal<{counter++}>", Styling.ListButton); SameLine();
            SetTooltipHovered(currentPetName);
            Label($"{nickname.Item2} ##internal<{counter++}>", Styling.ListButton);
            SetTooltipHovered(nickname.Item2);
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
        texturePointer = 0;

        BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeightBattle));
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 01f));
        ImGui.BeginTable("##Image Table 2", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY);
        DrawFillerBar(2);

        user.SerializableUser.LoopThrough(nickname =>
        {
            if (nickname.Item1 >= -1) return;

            SetColumn(0);
            texturePointer++;
            DrawTexture(ref texturePointer);
            SetColumn(1);

            if (currentIsLocalUser)
            {
                DrawAdvancedBarWithQuit("Nickname", nickname.Item2, ref counter,
                    () => PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForBattleID(nickname.Item1, true),
                    "X", "Clears the nickname!",
                    () =>
                    {
                        user.SerializableUser.RemoveNickname(nickname.Item1, true);
                        PluginLink.Configuration.Save();
                    });
            }
            else DrawBasicBar("Nickname", nickname.Item2, ref counter);

            string currentPetName = StringUtils.instance.MakeTitleCase(RemapUtils.instance.PetIDToName(nickname.Item1));
            DrawBasicBar("Pet Name", currentPetName, ref counter);
            DrawBasicBar("Pet ID", nickname.Item1.ToString(), ref counter);
            DrawRightHeading(2);
            DrawFillerBar(2);
        });
        ImGui.EndTable();
        ImGui.PopStyleVar();

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
        if (petMode != PetMode.ShareMode)
        {
            if (Button($"{StringUtils.instance.MakeTitleCase(user?.UserName ?? string.Empty)}", Styling.ListButton))
                SetUserMode(!userMode);
        }
        else
        {
            Label($"{StringUtils.instance.MakeTitleCase(user?.UserName ?? string.Empty)}", Styling.ListButton);
        }
        SetTooltipHovered($"Username: {StringUtils.instance.MakeTitleCase(user?.UserName ?? string.Empty)}\nClick to change user.");
        SameLine();
        Label($"{SheetUtils.instance.GetWorldName(user?.Homeworld ?? 9999)}", Styling.ListButton); SameLine();
        SetTooltipHovered($"Homeworld: {SheetUtils.instance.GetWorldName(user?.Homeworld ?? 9999)}");
        ImGui.EndListBox();
        NewLine();
    }

    void SetUserMode(bool userMode)
    {
        this.userMode = userMode;
        if (!userMode)
        {
            user = null!;
            youSureMode = false;
            youSureUser = null!;
        }
    }

    void SetAdvancedMode(bool advanced)
    {
        advancedMode = advanced;
        contains.Clear();

        if (!advancedMode) return;

        user = PluginLink.PettableUserHandler.LocalUser()!;
        importedUser = null!;
        IsOpen = true;
        SetPetMode(PetMode.ShareMode);
    }

    double exportTimer = 0;
    double errorTimer = 0;

    public void DrawExportHeader()
    {
        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        int counter = 30;
        BeginListBox("##<clipboard>", new System.Numerics.Vector2(318, 60));
        Label("A friend can import your code to see your names.", new System.Numerics.Vector2(310, 24));
        if (Button($"Export to Clipboard##clipboardExport{counter++}", Styling.ListButton))
        {
            if (ImGui.IsKeyDown(ImGuiKey.LeftShift) || PluginLink.Configuration.alwaysOpenAdvancedMode)
                SetAdvancedMode(true);
            else
            {
                SetAdvancedMode(false);
                FillList();
                Export();
            }
        }
        if (errorTimer > 0)
        {
            errorTimer -= PluginHandlers.Framework.UpdateDelta.Milliseconds * 0.001;
            SetTooltip("...[ERROR] [Couldn't export Nicknames]...");
        }
        else if (exportTimer > 0)
        {
            exportTimer -= PluginHandlers.Framework.UpdateDelta.Milliseconds * 0.001;
            SetTooltip("...[Exported Names]...");
        }
        else
        {
            SetTooltipHovered("Exports ALL your nicknames to a list.\nYou can send this list to anyone.\nFor example: Paste this text into Discord and let a friend copy it.\n\n[Hold L-Shift for advanced options.]");
        }
        SameLine();
        if (Button($"Import from Clipboard##clipboardImport{counter++}", Styling.ListButton))
        {
            SetAdvancedMode(false);
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

    List<TextureWrap> TextureWraps = new List<TextureWrap>();

    void DrawList()
    {
        int counter = 10;
        texturePointer = 0;

        BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeight), StylingColours.titleBg);
        NewLine();
        if (!_openedAddPet)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 01f));
            ImGui.BeginTable("##Image Table 1", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY);
            DrawFillerBar(2);

            if (!_openedAddPet && currentIsLocalUser)
            {
                SetColumn(0);
                DrawTexture(ref texturePointer);
                texturePointer++;
                SetColumn(1);
                TransparentLabel(Styling.ListButton);
                SameLinePretendSpace(); SameLinePretendSpace();
                TransparentLabel($"##<{counter++}>", new Vector2(480, 25));
                SameLinePretendSpace();

                if (currentIsLocalUser)
                {
                    if (XButton("+" + $"##<Close{counter++}>", Styling.SmallButton))
                        SetOpenedAddPet(true);
                    SetTooltipHovered($"Search for a new pet.");
                    SameLine();
                    NewLine();
                }
                DrawRightHeading(2);
                DrawFillerBar(2);
            }
            else texturePointer++;

            if (!currentIsLocalUser) SetOpenedAddPet(false);

            user?.SerializableUser?.LoopThrough(nickname =>
            {
                if (nickname.Item1 < 0) return;

                SetColumn(0);
                DrawTexture(ref texturePointer);
                texturePointer ++;
                SetColumn(1);

                if (currentIsLocalUser)
                {
                    DrawAdvancedBarWithQuit("Nickname", nickname.Item2, ref counter,
                        () => PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForId(nickname.Item1, true),
                        "X", "Deletes the nickname!",
                        () =>
                        {
                            user.SerializableUser.RemoveNickname(nickname.Item1, true);
                            PluginLink.Configuration.Save();
                        });
                }
                else DrawBasicBar("Nickname", nickname.Item2, ref counter);

                string currentPetName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.Item1));
                DrawBasicBar("Minion Name", currentPetName, ref counter);
                DrawBasicBar("Minion ID", nickname.Item1.ToString(), ref counter);
                DrawRightHeading(2);
                DrawFillerBar(2);

            });
            ImGui.EndTable();
            ImGui.PopStyleVar();
        }
        else DrawOpenedNewPet();

        ImGui.EndListBox();
    }

    void FillTextureList()
    {
        DisposeTextures();
        if (user == null) return;
        AddTexture(-1);
        user.SerializableUser.LoopThrough(nickname =>
        {
            if (petMode == PetMode.Normal && nickname.Item1 < 0) return;
            if (petMode == PetMode.BattlePet && nickname.Item1 > -2) return;
            AddTexture(nickname.Item1);
        });
    }

    void AddTexture(int ID)
    {
        uint textureID = RemapUtils.instance.GetTextureID(ID);
        if (textureID == 0) return;
        string iconPath = PluginHandlers.TextureProvider.GetIconPath(textureID)!;
        if (iconPath == null) return;
        TextureWrap textureWrap = PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!;
        TextureWraps!.Add(textureWrap);
    }

    void DrawRightHeading(int column)
    {
        SetColumn(column);
        TransparentLabel(new Vector2(5, 0));
    }

    void DrawTexture(ref int textureIndex)
    {
        nint texturePointer = nint.Zero;
        if (textureIndex < TextureWraps.Count)
            texturePointer = TextureWraps[textureIndex].ImGuiHandle;

        DrawTexture(texturePointer);
    }

    void DrawTexture(nint thenint)
    {
        TransparentLabel("", new Vector2(4, 0)); SameLineNoMargin();
        nint texturePointer = thenint;
        if (texturePointer != nint.Zero) ImGui.Image(texturePointer, new Vector2(83, 83));
        SameLineNoMargin(); TransparentLabel("", new Vector2(4, 0));
    }

    void DrawAdvancedBarWithQuit(string label, string value, ref int counter, Action callback, string quitText = "", string quitTooltip = "", Action callback2 = null)
    {
        DrawBasicLabel(label);
        if (Button($"          {value} ##<{counter++}>", new Vector2(480, 25))) callback?.Invoke();
        SetTooltipHovered($"{label}: {value.Trim()}");
        if (callback2 == null) return;
        SameLinePretendSpace();
        if (XButton(quitText + $"##<Close{counter++}>", Styling.SmallButton)) callback2?.Invoke();
        SetTooltipHovered(quitTooltip);
    }

    void DrawYesNoBar(string label, ref int counter, Action yesCallback, Action noCallback)
    {
        Label(label + $"##<{counter++}>", new Vector2(508, 25));
        SameLinePretendSpace(); SameLinePretendSpace();
        if (Button("Yes", Styling.ListIDField))
            yesCallback?.Invoke();
        SetTooltipHovered("Will delete this user and all their nicknames from your savefile!");
        SameLinePretendSpace();
        if (Button("No", Styling.ListIDField))
            noCallback?.Invoke();
        SetTooltipHovered("Will keep this user and all their nicknames saved to your savefile!");
    }

    void DrawBasicBar(string label, string value, ref int counter)
    {
        DrawBasicLabel(label);
        Label(value.ToString() + $"##<{counter++}>", new Vector2(508, 25));
        SetTooltipHovered($"{label}: {value}");
    }

    void DrawBasicLabel(string label)
    {
        Label(label, Styling.ListButton);
        SameLinePretendSpace(); SameLinePretendSpace();
    }

    void SetColumn(int column)
    {
        if (column == 0) ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(column);
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(StylingColours.titleBg));
    }

    void DrawFillerBar(int columns)
    {
        ImGui.TableNextRow();
        for (int i = 0; i < columns; i++)
        {
            ImGui.TableSetColumnIndex(i);
            ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 0, 1)));
        }
    }

    void DrawOpenedNewPet()
    {
        texturePointer = 0;

        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 01f));
        ImGui.BeginTable("##Image Table 3", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY);
        DrawFillerBar(2);
        SetColumn(0);
        DrawTexture(ref texturePointer);
        SetColumn(1);
        

        int counter = 0;

        DrawBasicLabel("Search by name or ID");

        if (InputTextMultiLine(string.Empty, ref minionSearchField, PluginConstants.ffxivNameSize, new Vector2(480, 25), ImGuiInputTextFlags.CallbackCompletion | ImGuiInputTextFlags.CtrlEnterForNewLine))
        {
            searchField = minionSearchField;
            foundNicknames = SheetUtils.instance.GetThoseThatContain(searchField);
            FillTextureListFound();
            foreach (SerializableNickname nickname in foundNicknames)
                nickname.Setup();
        }
        SetTooltipHovered($"Filter on Minion ID or Name.");

        SameLinePretendSpace();
        if (XButton("X##ForOpenedPet", Styling.SmallButton))
        {
            SetOpenedAddPet(false);
            foundNicknames = new List<SerializableNickname>();
        }
        SetTooltipHovered($"Stop looking for a new pet?");

        DrawRightHeading(2);
        DrawFillerBar(2);

        foreach (SerializableNickname nickname in foundNicknames)
        {
            SetColumn(0);
            texturePointer++;
            DrawTexture(ref texturePointer);
            SetColumn(1);

            string currentPetName = StringUtils.instance.MakeTitleCase(nickname.BaseName);

            DrawAdvancedBarWithQuit("Nickname", nickname.Name, ref counter,
                () => PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForId(nickname.ID, true),
                "+", $"Add: {currentPetName}",
                () =>
                {
                    SetOpenedAddPet(false);
                    PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForId(nickname.ID, true);
                });

            
            DrawBasicBar("Minion Name", currentPetName, ref counter);
            DrawBasicBar("Minion ID", nickname.ID.ToString(), ref counter);
            DrawRightHeading(2);
            DrawFillerBar(2);
        }

        ImGui.EndTable();
        ImGui.PopStyleVar();
    }

    public void SetOpenedAddPet(bool value)
    {
        searchField = string.Empty;
        if (value == false && _openedAddPet != false)
            FillTextureList();
        _openedAddPet = value;

    }

    void FillTextureListFound()
    {
        DisposeTextures();
        AddTexture(-1);
        foreach (SerializableNickname nName in foundNicknames)
            AddTexture(nName.ID);
    }

    protected override void OnDispose()
    {
        DisposeTextures();
    }

    void DisposeTextures()
    {
        lastUser = user;
        foreach (TextureWrap tWrap in TextureWraps)
            tWrap?.Dispose();
        TextureWraps?.Clear();
    }
}
