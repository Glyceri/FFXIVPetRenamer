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
using Dalamud.Game.Text;
using PetRenamer.Core.Networking.NetworkingElements;
using PetRenamer.Core.Sharing;
using PetRenamer.Core.Sharing.Importing;
using PetRenamer.Core.Sharing.Importing.Data;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
[ModeTogglePetWindow]
public class PetListWindow : PetWindow
{
    int maxBoxHeight = 675;
    int maxBoxHeightBattle = 631;
    int maxBoxHeightSharing = 655;

    public PetListWindow() : base("Pet Nicknames List", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(800, 783 + 64),
            MaximumSize = new Vector2(800, 783 + 64)
        };
    }

    int internalCounter = 0;

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
    bool existed = false;

    public override void OnDraw()
    {
        internalCounter = 0;
        if (PluginLink.Configuration.serializableUsersV3!.Length == 0) return;
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        PettableUser localUser = PluginLink.PettableUserHandler.LocalUser()!;
        if ((user ??= localUser!) == null) return;
        if (petMode != PetMode.Normal) SetOpenedAddPet(false);
        if (petMode != PetMode.ShareMode) 
        { 
            SetAdvancedMode(false);
            CleanupImport();
        }
        if (petMode == PetMode.ShareMode)
        {
            SetUserMode(false);
            user = localUser!;
        }
        if (user == null) return;
        currentIsLocalUser = user == localUser;
        if (!currentIsLocalUser) SetOpenedAddPet(false);

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
        DrawBasicUserHeader();
        if (userMode) DrawUserSelect();
        else DrawList();
    }

    void HandleOnDrawBattlePetList()
    {
        if (user == null) return;
        DrawBasicUserHeader();
        if (userMode) DrawUserSelect();
        else DrawBattlePetList();
    }

    void HandleOnDrawSharing()
    {
        if (user == null) return;
        
        if (advancedMode) DrawUserHeader(user, 0, 32, () => OverrideLabel("Advanced Exporting"));
        else if (importedData != null && existingUser != null) DrawUserHeader(existingUser, 0, 32, () => OverrideLabel("Importing"));
        else DrawBasicUserHeader();

        if (userMode) DrawUserSelect();
        else DrawSharing();
    }

    void DrawBasicUserHeader()
    {
        DrawUserHeader(user, 75, 15, DrawExportHeader);
    }

    void DrawUserSelect()
    {
        OverrideLabel("Help! I cannot see any profile pictures. Please enable: [Allow Automatic Profile Pictures] in the Settings menu.", Styling.FillSize);
        SameLinePretendSpace();
        if (XButton(SeIconChar.MouseWheel.ToIconString() + "##<SafetySettings>", Styling.SmallButton))
            PluginLink.WindowHandler.OpenWindow<ConfigWindow>();
        SetTooltipHovered("Open Settings Menu");
        BeginListBox("##<4>", new System.Numerics.Vector2(780, maxBoxHeightBattle), StylingColours.titleBg);
        bool buttonPressed = false;
        int counter = 1000;
        texturePointer = 0;

        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 01f));
        ImGui.BeginTable("##Image Table 4", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY);
        DrawFillerBar(3);

        foreach (PettableUser u in PluginLink.PettableUserHandler.Users)
        {
            if (u == null) continue;
            SetColumn(0);
            DrawUserTexture(u);

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
                "X", $"Remove User: {u.UserName}@{SheetUtils.instance.GetWorldName(u.Homeworld)}", () =>
                {
                    youSureMode = !youSureMode;
                    youSureUser = youSureMode ? u : null!;
                });
            }

            if (youSureMode && youSureUser == u)
            {
                TransparentLabel("", new Vector2(1, 25));
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
                    });
            }
            else
            {
                DrawBasicBar("Homeworld", $"{SheetUtils.instance.GetWorldName(u.Homeworld)}", ref counter);
                DrawBasicBar("Petcount", $"Total: {u.SerializableUser.AccurateTotalPetCount()}, Minions: {u.SerializableUser.AccurateMinionCount()}, Battle Pets: {u.SerializableUser.AccurateBattlePetCount()}", ref counter);
            }
            DrawRightHeading(2);
            DrawFillerBar(3);

            if (buttonPressed) break;
        }

        ImGui.EndTable();
        ImGui.PopStyleVar();

        ImGui.EndListBox();
    }

    SucceededImportData importedData = null!;
    PettableUser existingUser = null!;
    PettableUser removeMeUser = null!;

    unsafe void DrawSharing()
    {
        if (user == null) return;

        if (advancedMode)
        {
            DrawAdvancedSharing();
            return;
        }

        if (importedData == null) return;
        if (existingUser == null)
        {
            existingUser = PluginLink.PettableUserHandler.GetUser(importedData.UserName, importedData.HomeWorld)!;
            removeMeUser = PluginLink.PettableUserHandler.GetUser(importedData.UserName, importedData.HomeWorld)!;
            FillTextureList(new PettableUser("_temp_", 0, new SerializableUserV3(importedData.ids, importedData.names, "_temp_", 0)));
        }
        DrawListSharing();
        DrawFooterSharing();
    }

    void DrawAdvancedSharing()
    {
        DrawAdvancedList();
    }

    bool current = true;
    void DrawAdvancedList()
    {
        if (!SharingHandler.HasSetup(user) && user.SerializableUser.length != 0)
        {
            SharingHandler.SetupList(user);
            current = true;
        }

        BeginListBox("##<6>", new System.Numerics.Vector2(780, maxBoxHeightSharing));

        int texturePointer = 0;
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 01f));
        ImGui.BeginTable("##Image Table 6", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY);
        DrawFillerBar(3);
        ImGui.TableNextRow();

        SetColumn(0);
        DrawTexture(TextureWraps[texturePointer++].ImGuiHandle, () =>
        {
            if (Checkbox($"##{internalCounter++}", "Toggle all", ref current))
                SharingHandler.ToggleAll(current);
        });

        SetColumn(1);
        DrawEmptyBar("X", "Stop Sharing", () => advancedMode = false);

        DrawRightHeading(2);
        DrawFillerBar(3);

        for (int i = 0; i < user.SerializableUser.length; i++)
        {
            ImGui.TableNextRow();
            SetColumn(0);
            if (texturePointer < TextureWraps.Count)
            {
                DrawTexture(TextureWraps[texturePointer++].ImGuiHandle, () =>
                {
                    current = SharingHandler.GetContainsList(i);
                    Checkbox($"##{internalCounter++}", ref current);
                    SetTooltipHovered("Include pet in export");
                    SharingHandler.SetContainsList(i, current);
                });
            }
            SetColumn(1);

            DrawBasicBar("Nickname", $"{user.SerializableUser.names[i]}", ref internalCounter);
            string basePetName =
                user.SerializableUser.ids[i] < -1 ?
                RemapUtils.instance.PetIDToName(user.SerializableUser.ids[i]) :
                SheetUtils.instance.GetPetName(user.SerializableUser.ids[i]);
            DrawBasicBar("Type", $"{basePetName}", ref internalCounter);
            DrawBasicBar("ID", $"{user.SerializableUser.ids[i]}", ref internalCounter);

            DrawRightHeading(2);
            DrawFillerBar(3);
        }

        ImGui.EndTable();
        ImGui.PopStyleVar();

        ImGui.EndListBox();

        NewLine();
        ImGui.SameLine(638);

        if (Button($"Export Nicknames List##EmportListSave", Styling.ListButton))
        {
            SharingHandler.Export(user);
            SetAdvancedMode(false);
        }
    }

    void DrawFooterSharing()
    {
        if (existingUser == null) return;
        NewLine();
        ImGui.SameLine(638);

        if (Button($"Save Imported List##importListSave", Styling.ListButton))
        {
            for (int i = 0; i < importedData.ids.Length; i++)
            {
                if (importedData.importTypes[i] == ImportType.Remove) existingUser.SerializableUser.RemoveNickname(importedData.ids[i]);
                else existingUser.SerializableUser.SaveNickname(importedData.ids[i], importedData.names[i], importedData.importTypes[i] == ImportType.New || importedData.importTypes[i] == ImportType.Rename, false);
            }
            
            PenumbraIPCProvider.RedrawBattlePetByIndex(existingUser.BattlePetIndex);
            PenumbraIPCProvider.RedrawMinionByIndex(existingUser.MinionIndex);

            removeMeUser = null!;
            existingUser = null!;
            importedData = null!;

            PluginLink.Configuration.Save();
        }
    }

    void DrawListSharing()
    {
        texturePointer = 0;
        BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeightSharing));

        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 01f));
        ImGui.BeginTable("##Image Table 7", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY);
        DrawFillerBar(3);

        for (int i = 0; i < importedData.ids.Length; i++)
        {
            if (importedData.names[i] == string.Empty) continue;
            ImGui.TableNextRow();
            SetColumn(0);
            if (texturePointer < TextureWraps.Count -1)
            {
                DrawTexture(TextureWraps[++texturePointer].ImGuiHandle, () => DrawImportType(importedData.importTypes[i]));
            }
            SetColumn(1);

            string name = importedData.names[i];
            DrawBasicBar("Nickname", $"{name}", ref internalCounter);
            int id = importedData.ids[i];
            string basePetName = id < -1 ? RemapUtils.instance.PetIDToName(id) : SheetUtils.instance.GetPetName(id);
            DrawBasicBar("Type", $"{basePetName}", ref internalCounter);
            DrawBasicBar("ID", $"{id}", ref internalCounter);

            DrawRightHeading(2);
            DrawFillerBar(3);
        }

        ImGui.EndTable();
        ImGui.PopStyleVar();

        ImGui.EndListBox();
    }

    void DrawImportType(ImportType importType)
    {
        if (importType == ImportType.None) Label("=", Styling.SmallButton);
        if (importType == ImportType.New) NewLabel("+", Styling.SmallButton);
        if (importType == ImportType.Rename) NewLabel("O", Styling.SmallButton);
        if (importType == ImportType.Remove) XButtonError("X", Styling.SmallButton);
    }

    void DrawBattlePetList()
    {
        DrawBattlePetWarningHeader();
        int counter = 10;
        texturePointer = 0;

        BeginListBox("##<2>", new System.Numerics.Vector2(780, maxBoxHeightBattle));
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 01f));
        ImGui.BeginTable("##Image Table 2", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY);
        DrawFillerBar(3);

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
            DrawFillerBar(3);
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

    void DrawUserHeader(PettableUser user, float xOffset, float yOffset, Action drawMidElement)
    {
        if (user == null) return;
        BeginListBox($"##<PetList{internalCounter++}>", new System.Numerics.Vector2(780, 90), StylingColours.titleBg);

        DrawUserTexture(user);

        SameLinePretendSpace();

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
        ImGui.SetCursorPos(ImGui.GetCursorPos() - new System.Numerics.Vector2(Styling.ListButton.X + 8, -Styling.ListButton.Y - 4));
        OverrideLabel($"{SheetUtils.instance.GetWorldName(user?.Homeworld ?? 9999)}", Styling.ListButton); SameLine();
        SetTooltipHovered($"Homeworld: {SheetUtils.instance.GetWorldName(user?.Homeworld ?? 9999)}");

        ImGui.SetCursorPos(ImGui.GetCursorPos() - new System.Numerics.Vector2(Styling.ListButton.X + 8, -Styling.ListButton.Y * 2 - 8));

        OverrideLabel($"[{user?.SerializableUser?.AccurateTotalPetCount()}, {user?.SerializableUser?.AccurateMinionCount()}, {user?.SerializableUser?.AccurateBattlePetCount()}]", Styling.ListButton);
        SetTooltipHovered($"Total Pet Count: {user?.SerializableUser?.AccurateTotalPetCount()}, Minion Count: {user?.SerializableUser?.AccurateMinionCount()}, Battle Pet Count: {user?.SerializableUser?.AccurateBattlePetCount()}");

        ImGui.EndListBox();

        if (drawMidElement != null)
        {
            SameLine();
            ImGui.SetItemAllowOverlap();
            ImGui.SetCursorPos(ImGui.GetCursorPos() - new System.Numerics.Vector2(350 + xOffset, -yOffset));
            drawMidElement?.Invoke();
        }
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
        SharingHandler.ClearList();

        if (!advancedMode) return;

        user = PluginLink.PettableUserHandler.LocalUser()!;
        CleanupImport();
        IsOpen = true;
        SetPetMode(PetMode.ShareMode);
    }
    
    void CleanupImport()
    {
        importedData = null!;
        existingUser = null!;
        if (removeMeUser != null)
        {
            if(!existed) PluginLink.PettableUserHandler.DeclareUser(removeMeUser.SerializableUser, Core.PettableUserSystem.Enums.UserDeclareType.Remove);
            existed = false;
            removeMeUser = null!;
        }
    }

    public void DrawExportHeader()
    {
        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        int counter = 30;
        BeginListBox($"##<clipboard>{internalCounter++}", new System.Numerics.Vector2(318, 60));
        OverrideLabel("A friend can import your code to see your names.", new System.Numerics.Vector2(310, 24));
        if (Button($"Export to Clipboard##clipboardExport{counter++}", Styling.ListButton))
        {
            if (ImGui.IsKeyDown(ImGuiKey.LeftShift) || PluginLink.Configuration.alwaysOpenAdvancedMode) SetAdvancedMode(true);
            else
            {
                SetAdvancedMode(false);
                SharingHandler.SetupList(user);
                SharingHandler.Export(user);
            }
        }
        SetTooltipHovered("Exports ALL your nicknames to a list.\nYou can send this list to anyone.\nFor example: Paste this text into Discord and let a friend copy it.\n\n[Hold L-Shift for advanced options.]");

        SameLine();
        if (Button($"Import from Clipboard##clipboardImport{counter++}", Styling.ListButton))
        {
            SetAdvancedMode(false);
            ImportData data = ImportHandler.SetImportString(ImGui.GetClipboardText());
            if (data is FailedImportData failedImportData) PluginLog.Log($"Import Error occured: {failedImportData.ErrorMessage}");
            if (data is SucceededImportData succeededImportData)
            {
                importedData = succeededImportData;
                SetPetMode(PetMode.ShareMode);
                PetListWindow window = PluginLink.WindowHandler.GetWindow<PetListWindow>();
                window.IsOpen = true;

                existed = PluginLink.PettableUserHandler.GetUser(importedData.UserName, importedData.HomeWorld) != null;
                PluginLink.PettableUserHandler.DeclareUser(new SerializableUserV3(importedData.UserName, importedData.HomeWorld), Core.PettableUserSystem.Enums.UserDeclareType.Add);
            }
        }
        SetTooltipHovered("After having copied a list of names from a friend.\nClicking this button will result into importing all their nicknames \nallowing you to see them for yourself.");
        ImGui.EndListBox();
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
            DrawFillerBar(3);

            if (!_openedAddPet && currentIsLocalUser)
            {
                SetColumn(0);
                DrawTexture(ref texturePointer);
                texturePointer++;
                SetColumn(1);
                DrawEmptyBar("+", "Search for a new pet.", () => SetOpenedAddPet(true), currentIsLocalUser);

                DrawRightHeading(2);
                DrawFillerBar(3);
            }
            else texturePointer++;

            if (!currentIsLocalUser) SetOpenedAddPet(false);

            user?.SerializableUser?.LoopThrough(nickname =>
            {
                if (nickname.Item1 < 0) return;

                SetColumn(0);
                DrawTexture(ref texturePointer);
                texturePointer++;
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
                DrawFillerBar(3);

            });
            ImGui.EndTable();
            ImGui.PopStyleVar();
        }
        else DrawOpenedNewPet();

        ImGui.EndListBox();
    }

    void FillTextureList(PettableUser user = null!)
    {
        DisposeTextures();
        if (user == null) user = this.user;
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
        TransparentLabel(new Vector2(25, 0));
    }

    void DrawTexture(ref int textureIndex)
    {
        nint texturePointer = nint.Zero;
        if (textureIndex < TextureWraps.Count)
            texturePointer = TextureWraps[textureIndex].ImGuiHandle;

        DrawTexture(texturePointer);
    }

    void DrawTexture(nint theint, Action drawExtraButton)
    {
        DrawTexture(theint);
        DrawRedownloadButton(drawExtraButton);
    }

    void DrawUserTexture(PettableUser u)
    {
        DrawTexture(u, () => DrawRedownloadButton(u));
    }

    void DrawTexture(PettableUser u, Action drawExtraButton)
    {
        DrawTexture(ProfilePictureNetworked.instance.GetTexture(u));
        DrawRedownloadButton(drawExtraButton);
    }

    void DrawTexture(nint thenint)
    {
        TransparentLabel("", new Vector2(4, 0)); SameLineNoMargin();
        nint texturePointer = thenint;
        if (!PluginLink.Configuration.displayImages)
        {
            PushStyleColor(ImGuiCol.Button, StylingColours.defaultBackground);
            PushStyleColor(ImGuiCol.ButtonActive, StylingColours.defaultBackground);
            PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.defaultBackground);
            ImGui.Button("", new System.Numerics.Vector2(83, 83));
        }
        else
        {
            if (texturePointer != nint.Zero) ImGui.Image(texturePointer, new Vector2(83, 83));
        }
        SameLineNoMargin(); TransparentLabel("", new Vector2(4, 0));
    }

    void DrawRedownloadButton(Action drawMe)
    {
        if (drawMe == null) return;
        ImGui.SetItemAllowOverlap();
        ImGui.SameLine();
        ImGui.SetCursorPos(ImGui.GetCursorPos() - new System.Numerics.Vector2(35, -59));
        drawMe?.Invoke();
    }

    void DrawRedownloadButton(PettableUser u)
    {
        if (RedownloadButton(SeIconChar.QuestSync.ToIconString() + $"##<Redownload>{internalCounter++}", Styling.SmallButton))
            ProfilePictureNetworked.instance.RequestDownload((u.UserName, u.Homeworld));
        SetTooltipHovered($"Redownload profile picture for: {u.UserName}@{SheetUtils.instance.GetWorldName(u.Homeworld)}");
    }

    void DrawAdvancedBarWithQuit(string label, string value, ref int counter, Action callback, string quitText = "", string quitTooltip = "", Action callback2 = null!)
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
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(StylingColours.listBox));
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

    void DrawEmptyBar(string buttonString, string tooltipString, Action callback, bool check = true)
    {
        TransparentLabel(Styling.ListButton);
        SameLinePretendSpace(); SameLinePretendSpace();
        TransparentLabel($"##<Transparent{internalCounter++}>", new Vector2(480, 25));
        SameLinePretendSpace();

        if (!check) return;

        if (XButton(buttonString + $"##<Close{internalCounter++}>", Styling.SmallButton))
            callback?.Invoke();
        SetTooltipHovered(tooltipString);
        SameLine();
        NewLine();

    }

    void DrawOpenedNewPet()
    {
        texturePointer = 0;

        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 01f));
        ImGui.BeginTable("##Image Table 3", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY);
        DrawFillerBar(3);
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
        DrawFillerBar(3);

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
            DrawFillerBar(3);
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
