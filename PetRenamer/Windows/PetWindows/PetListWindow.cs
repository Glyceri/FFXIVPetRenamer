using Dalamud.Game.Text;
using Dalamud.Interface.Internal;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Ipc.PenumbraIPCHelper;
using PetRenamer.Core.Networking.NetworkingElements;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Sharing;
using PetRenamer.Core.Sharing.Importing;
using PetRenamer.Core.Sharing.Importing.Data;
using PetRenamer.Logging;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System;
using System.Collections.Generic;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
[ModeTogglePetWindow]
public class PetListWindow : PetWindow
{
    readonly int maxBoxHeight = 675;
    readonly int maxBoxHeightBattle = 631;
    readonly int maxBoxHeightSharing = 655;

    public PetListWindow() : base("Pet Nicknames List", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(800, 783 + 64),
            MaximumSize = new Vector2(800, 783 + 64)
        };
    }

    int texturePointer = 0;
    int graphPointer = -1;

    PettableUser user = null!;
    PettableUser lastUser = null!;
    PettableUser temporaryUser = null!;

    PettableUser existingUser = null!;
    PettableUser removeMeUser = null!;
    PettableUser youSureUser = null!;

    bool userMode = false;
    bool currentIsLocalUser = false;
    bool current = true;
    bool releaseGraph = false;
    bool _openedAddPet = false;
    bool youSureMode = false;
    bool advancedMode = false;
    bool existed = false;

    string searchField = string.Empty;
    string minionSearchField = string.Empty;

    List<SerializableNickname> foundNicknames = new List<SerializableNickname>();
    readonly List<IDalamudTextureWrap> TextureWraps = new List<IDalamudTextureWrap>();

    SucceededImportData importedData = null!;

    public override void OnDraw()
    {
        if (PluginLink.Configuration.serializableUsersV3!.Length == 0) return;
        if (PluginHandlers.ClientState.LocalPlayer! == null) return;
        PettableUser localUser = PluginLink.PettableUserHandler.LocalUser()!;
        if ((user ??= localUser!) == null) return;
        HandleModeCleanups(localUser);
    }

    void HandleModeCleanups(PettableUser localUser)
    {
        if (localUser == null!) return;
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

        temporaryUser ??= new PettableUser(user.UserName, user.Homeworld, new SerializableUserV3(user.UserName, user.Homeworld));

        currentIsLocalUser = user.UserName == localUser.UserName && user.Homeworld == localUser.Homeworld;
        if (!currentIsLocalUser) SetOpenedAddPet(false);

        if (user.UserChanged || user != lastUser)
            FillTextureList();
    }

    public override unsafe void OnDrawNormal() => HandleOnDrawNormal();
    public override unsafe void OnDrawBattlePet() => HandleOnDrawBattlePetList();
    public override unsafe void OnDrawSharing() => HandleOnDrawSharing();
    internal override void OnPetModeChange(PetMode mode) => FillTextureList();

    void HandleOnDrawNormal() => DrawHelper(DrawBasicUserHeader, DrawUserSelect, DrawList);
    void HandleOnDrawBattlePetList() => DrawHelper(DrawBasicUserHeader, DrawUserSelect, DrawBattlePetList);
    void HandleOnDrawSharing() => DrawHelper(SharingHeader, DrawUserSelect, DrawSharing);
    void DrawBasicUserHeader() => DrawUserHeader(user, 75, 15, DrawExportHeader);

    void SharingHeader()
    {
        if (advancedMode) DrawUserHeader(user, 0, 32, () => OverrideLabel("Advanced Exporting", new Vector2(200, BarSize)));
        else if (importedData != null && existingUser != null) DrawUserHeader(existingUser, 0, 32, () => OverrideLabel("Importing", new Vector2(200, BarSize)));
        else DrawBasicUserHeader();
    }

    void DrawHelper(Action header, Action userSelect, Action list)
    {
        if (user == null) return;
        header();
        if (userMode) userSelect();
        else list();
    }

    void DrawUserSelect()
    {
        DrawWarningLabel(PluginConstants.Strings.userListPfpWarning, SeIconChar.MouseWheel.ToIconString(), "Open Settings Menu", PluginLink.WindowHandler.OpenWindow<ConfigWindow>);
        PettableUser curUser = null!;

        if (!BeginListBox("##<4>", new System.Numerics.Vector2(780, maxBoxHeightBattle)))
            return;

        DrawGraph(new List<Action<int, int>>()
        {
            (row, column) => UserColumnOne(curUser),
            (row, column) => UserColumnTwo(curUser),
            (row, column) => DrawRightHeading(column)
        }, () =>
        {
            bool returner = ++graphPointer < PluginLink.PettableUserHandler.Users.Count;
            if (returner) curUser = PluginLink.PettableUserHandler.Users[graphPointer];
            else curUser = null!;
            return returner;
        });
        ImGui.EndListBox();
    }

    void UserColumnOne(PettableUser user) => DrawUserTexture(user);
    void UserColumnTwo(PettableUser u)
    {
        Action callback = u.LocalUser ? null! : () => ToggleSureMode(u);

        string userText = $"Remove User: {u.UserName}@{SheetUtils.instance.GetWorldName(u.Homeworld)}";
        if (u.IsIPCOnlyUser) userText += "\n(This user will NEVER get saved)";
        DrawAdvancedBarWithQuit("Username", u.UserName, () =>
            {
                SetUserMode(false);
                user = u;
            }, "X", userText, callback, u.IsIPCOnlyUser);

        if (youSureMode && youSureUser == u) DrawYesNoBar($"Are you sure you want to remove: {youSureUser.UserName}@{SheetUtils.instance.GetWorldName(youSureUser.Homeworld)}", () => DeleteUser(u), DisableYouSureMode);
        else
        {
            DrawBasicBar("Homeworld", $"{SheetUtils.instance.GetWorldName(u.Homeworld)}", u.IsIPCOnlyUser);
            DrawBasicBar("Petcount", $"Total: {u.SerializableUser.AccurateTotalPetCount()}, Minions: {u.SerializableUser.AccurateMinionCount()}, Battle Pets: {u.SerializableUser.AccurateBattlePetCount()}", u.IsIPCOnlyUser);
        }
    }

    void DeleteUser(PettableUser u)
    {
        DisableYouSureMode();
        PluginLink.PettableUserHandler.DeclareUser(u.SerializableUser, Core.PettableUserSystem.Enums.UserDeclareType.Remove);
        PluginLink.Configuration.Save();
    }

    void DrawSharing()
    {
        if (user == null) return;

        if (advancedMode)
        {
            DrawAdvancedList();
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

    void DrawAdvancedList()
    {
        if (!SharingHandler.HasSetup(user) && user.SerializableUser.length != 0)
        {
            SharingHandler.SetupList(user);
            FillTextureList();
            current = true;
        }

        BeginListBox("##<6>", new System.Numerics.Vector2(ContentAvailableX, maxBoxHeightSharing));

        DrawPetGraph(user, () =>
        {
            int index = graphPointer - 1;
            if (index == -1)
            {
                if (Checkbox($"##{internalCounter++}", "Toggle all", ref current))
                    SharingHandler.ToggleAll(current);
                return;
            }
            current = SharingHandler.GetContainsList(index);
            Checkbox($"##{internalCounter++}", ref current);
            SetTooltipHovered("Include pet in export");
            SharingHandler.SetContainsList(index, current);
        },
        (row, column) =>
        {
            int index = graphPointer - 1;
            if (index == -1)
            {
                DrawEmptyBar("X", "Stop Sharing", () => advancedMode = false);
                return;
            }

            QuickName pet = user.SerializableUser[index];

            DrawBasicBar("Nickname", $"{pet.Name}");
            string basePetName = pet.ID < -1 ?
                RemapUtils.instance.PetIDToName(pet.ID) :
                SheetUtils.instance.GetPetName(pet.ID);
            basePetName = StringUtils.instance.MakeTitleCase(basePetName);
            DrawBasicBar("Type", $"{basePetName}");
            DrawBasicBar("ID", $"{pet.ID}");

        }, () => false);
        ImGui.EndListBox();

        SpaceBottomRightButton();

        if (Button($"Export Nicknames List##EmportListSave", Styling.ListButton))
        {
            SharingHandler.Export(user);
            SetAdvancedMode(false);
        }
    }

    void DrawFooterSharing()
    {
        if (existingUser == null) return;
        SpaceBottomRightButton();

        if (Button($"Save Imported List##importListSave", Styling.ListButton))
        {
            for (int i = 0; i < importedData.ids.Length; i++)
            {
                if (importedData.importTypes[i] == ImportType.Remove) existingUser.SerializableUser.RemoveNickname(importedData.ids[i]);
                else existingUser.SerializableUser.SaveNickname(importedData.ids[i], importedData.names[i], importedData.importTypes[i] == ImportType.New || importedData.importTypes[i] == ImportType.Rename, false);
            }

            if (existingUser.LocalUser)
                IpcUtils.instance.SendAllData();

            PenumbraIPCProvider.RedrawBattlePetByIndex(existingUser.BattlePet.Index);
            PenumbraIPCProvider.RedrawMinionByIndex(existingUser.Minion.Index);

            removeMeUser = null!;
            existingUser = null!;
            importedData = null!;

            PluginLink.Configuration.Save();
        }
    }

    void DrawListSharing()
    {
        texturePointer = 0;
        BeginListBox("##<111>", new System.Numerics.Vector2(ContentAvailableX, maxBoxHeightSharing));

        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 01f));
        ImGui.BeginTable("##Image Table 7", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY);
        DrawFillerBar(3);

        for (int i = 0; i < importedData.ids.Length; i++)
        {
            if (importedData.names[i] == string.Empty) continue;
            ImGui.TableNextRow();
            SetColumn(0);
            if (texturePointer < TextureWraps.Count - 1)
            {
                DrawTexture(TextureWraps[++texturePointer].ImGuiHandle, () => DrawImportType(importedData.importTypes[i]));
            }
            SetColumn(1);

            string name = importedData.names[i];
            DrawBasicBar("Nickname", $"{name}");
            int id = importedData.ids[i];
            string basePetName = id < -1 ? RemapUtils.instance.PetIDToName(id) : SheetUtils.instance.GetPetName(id);
            basePetName = StringUtils.instance.MakeTitleCase(basePetName);
            DrawBasicBar("Type", $"{basePetName}");
            DrawBasicBar("ID", $"{id}");

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
        BeginListBox("##<2>", new System.Numerics.Vector2(ContentAvailableX, maxBoxHeightBattle));

        DrawPetGraph(user, null!, (row, column) =>
        {
            int index = graphPointer - 1;
            if (index < 0) return;

            QuickName nickname = user.SerializableUser[index];
            string infoString = nickname.HasIPCName ? "IPC Nickname" : "Nickname";

            if (currentIsLocalUser)
            {
                DrawAdvancedBarWithQuit(infoString, nickname.Name,
                    () => PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForId(nickname.ID, true),
                    "X", nickname.HasIPCName ? "Clears the IPC nickname!" : "Clears the nickname!",
                    () =>
                    {
                        user.SerializableUser.SaveNickname(nickname.ID, string.Empty, true, false, nickname.HasIPCName);
                        IpcUtils.instance.NotifyChange(nickname.ID, string.Empty);

                        PluginLink.Configuration.Save();
                    }, nickname.HasIPCName);
            }
            else DrawBasicBar(infoString, nickname.Name, nickname.HasIPCName);
            DrawFinalBars(StringUtils.instance.MakeTitleCase(RemapUtils.instance.PetIDToName(nickname.ID)), nickname.ID.ToString(), "Pet Name", "Pet ID", nickname.HasIPCName);
        },
        () =>
        {
            if (graphPointer == 0) return true;
            if (graphPointer - 1 >= 0) return user.SerializableUser[graphPointer - 1].ID >= -1;
            return false;
        });

        ImGui.EndListBox();
    }

    void DrawBattlePetWarningHeader()
    {
        BeginListBox("##WarningHeader", new System.Numerics.Vector2(ContentAvailableX, 40));
        ImGui.TextColored(StylingColours.defaultText, "Please note: /petglamour,/petmirage now works. You can no longer name pets based on your job.\nIf you, for example, name a ruby carbuncle they will have that same name regardless of job.");
        ImGui.EndListBox();
    }

    void DrawUserHeader(PettableUser user, float xOffset, float yOffset, Action drawMidElement)
    {
        if (user == null) return;
        if (!BeginListBox($"##<PetList{internalCounter++}>", new System.Numerics.Vector2(ContentAvailableX, 90)))
            return;

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
        SetTooltipHovered($"Username: {StringUtils.instance.MakeTitleCase(user?.UserName ?? string.Empty)}\nClick to change user."); SameLine();
        ImGui.SetCursorPos(ImGui.GetCursorPos() - new System.Numerics.Vector2(Styling.ListButton.X + 8, -Styling.ListButton.Y - 4));
        OverrideLabel($"{SheetUtils.instance.GetWorldName(user?.Homeworld ?? 9999)}", Styling.ListButton); SameLine();
        SetTooltipHovered($"Homeworld: {SheetUtils.instance.GetWorldName(user?.Homeworld ?? 9999)}");

        ImGui.SetCursorPos(ImGui.GetCursorPos() - new System.Numerics.Vector2(Styling.ListButton.X + 8, -Styling.ListButton.Y * 2 - 8));

        if (user!.IsIPCOnlyUser)
        {
            OverrideLabel($"[User added via IPC]", Styling.ListButton);
            SetTooltipHovered($"This user is added via the IPC. It will NOT get saved!");
        }
        else
        {
            OverrideLabel($"[{user?.SerializableUser?.AccurateTotalPetCount()}, {user?.SerializableUser?.AccurateMinionCount()}, {user?.SerializableUser?.AccurateBattlePetCount()}]", Styling.ListButton);
            SetTooltipHovered($"Total Pet Count: {user?.SerializableUser?.AccurateTotalPetCount()}, Minion Count: {user?.SerializableUser?.AccurateMinionCount()}, Battle Pet Count: {user?.SerializableUser?.AccurateBattlePetCount()}");
        }

        ImGui.EndListBox();

        if (drawMidElement != null)
        {
            SameLine();
            ImGui.SetItemAllowOverlap();
            ImGui.SetCursorPos(ImGui.GetCursorPos() - new System.Numerics.Vector2(350 + xOffset, -yOffset));
            drawMidElement?.Invoke();
        }
    }

    void ToggleSureMode(PettableUser u)
    {
        youSureMode = !youSureMode;
        youSureUser = youSureMode ? u : null!;
    }

    void SetUserMode(bool userMode)
    {
        this.userMode = userMode;
        if (!userMode)
        {
            user = null!;
            DisableYouSureMode();
        }
    }

    void DisableYouSureMode()
    {
        youSureMode = false;
        youSureUser = null!;
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
            if (!existed) PluginLink.PettableUserHandler.DeclareUser(removeMeUser.SerializableUser, Core.PettableUserSystem.Enums.UserDeclareType.Remove);
            existed = false;
            removeMeUser = null!;
        }
    }

    public void DrawExportHeader()
    {
        user ??= PluginLink.PettableUserHandler.LocalUser()!;

        BeginListBox($"##<clipboard>{internalCounter++}", new System.Numerics.Vector2(318, 60));
        OverrideLabel("A friend can import your code to see your names.", new System.Numerics.Vector2(310, 24));
        Button($"Export to Clipboard##clipboardExport{internalCounter++}", Styling.ListButton, PluginConstants.Strings.exportTooltip, ExportClipboard); SameLine();
        Button($"Import from Clipboard##clipboardImport{internalCounter++}", Styling.ListButton, PluginConstants.Strings.importTooltip, ImportClipboard);
        ImGui.EndListBox();
    }

    void ExportClipboard()
    {
        if (ImGui.IsKeyDown(ImGuiKey.LeftShift) || PluginLink.Configuration.alwaysOpenAdvancedMode) SetAdvancedMode(true);
        else
        {
            SetAdvancedMode(false);
            SharingHandler.SetupList(user);
            SharingHandler.Export(user);
        }
    }

    void ImportClipboard()
    {
        SetAdvancedMode(false);
        ImportData data = ImportHandler.SetImportString(ImGui.GetClipboardText());
        if (data is FailedImportData failedImportData) PetLog.Log($"Import Error occured: {failedImportData.ErrorMessage}");
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

    void SetSearch()
    {
        searchField = minionSearchField;
        foundNicknames = SheetUtils.instance.GetThoseThatContain(searchField);
        FillTextureListFound();
        temporaryUser.SerializableUser.Reset();
        foreach (SerializableNickname n in foundNicknames)
        {
            if (temporaryUser.SerializableUser.Contains(n.ID)) continue;
            AddTexture(n.ID);
            temporaryUser.SerializableUser.SaveNickname(n.ID, n.Name, false, true);
        }
    }

    void DrawList()
    {
        if (!BeginListBox("##<2>", new System.Numerics.Vector2(ContentAvailableX, maxBoxHeight)))
            return;

        DrawPetGraph(user, null!, (row, column) =>
        {
            int index = graphPointer - 1;

            if (row == 0 && currentIsLocalUser)
            {
                if (!_openedAddPet) DrawEmptyBar("+", "Search for a new pet.", () => SetOpenedAddPet(true), currentIsLocalUser);
                else
                {
                    DrawBasicLabel("Search by name or ID");
                    if (InputTextMultiLine(string.Empty, ref minionSearchField, PluginConstants.ffxivNameSize, new Vector2(480, 25), ImGuiInputTextFlags.CtrlEnterForNewLine, $"Filter on Minion ID or Name.")) SetSearch();
                    SameLinePretendSpace(); XButton("X##ForOpenedPet", Styling.SmallButton, "Stop looking for a new pet?", () => SetOpenedAddPet(false));
                }
            }
            if (index < 0) return;

            QuickName nickname = user.SerializableUser[index];

            string infoString = nickname.HasIPCName ? "IPC Nickname" : "Nickname";

            if (!currentIsLocalUser)
            {
                SetOpenedAddPet(false);
                DrawBasicBar(infoString, nickname.Name, nickname.HasIPCName);
            }
            else
            {
                string closeString = _openedAddPet ? "+" : "X";
                string tooltipString = _openedAddPet ? "Adds the minion" : nickname.HasIPCName ? "Clears the IPC Name" : "Deletes the nickname!";

                DrawAdvancedBarWithQuit(infoString, _openedAddPet ? nickname.RawName : nickname.Name,
                () => OpenID(nickname.ID, true),
                closeString, tooltipString,
                () =>
                {
                    OpenID(nickname.ID, false);
                    if (_openedAddPet) return;

                    if (nickname.HasIPCName) user.SerializableUser.SaveNickname(nickname.ID, string.Empty, true, false, true);
                    else user.SerializableUser.RemoveNickname(nickname.ID);
                    IpcUtils.instance.NotifyChange(nickname.ID, string.Empty);

                    PluginLink.Configuration.Save();
                }, nickname.HasIPCName);
            }
            DrawFinalBars(StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.ID)), nickname.ID.ToString(), "Minion Name", "Minion ID", nickname.HasIPCName);
        },
        () =>
        {
            if (graphPointer == 0 && !currentIsLocalUser) return true;
            if (graphPointer - 1 >= 0) return user.SerializableUser[graphPointer - 1].ID <= -1;
            return false;
        });

        ImGui.EndListBox();
    }

    void DrawFinalBars(string curName, string ID, string nameLabel, string IDLabel, bool isIPC = false)
    {
        DrawBasicBar(nameLabel, curName, isIPC);
        DrawBasicBar(IDLabel, ID, isIPC);
    }

    void OpenID(int id, bool force)
    {
        SetOpenedAddPet(false);
        PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForId(id, force);
    }

    void FillTextureList(PettableUser user = null!)
    {
        releaseGraph = true;
        DisposeTextures();
        AddTexture(-1);
        if ((user ??= this.user) == null) return;
        user.SerializableUser.LoopThrough(nickname => AddTexture(nickname.Item1));
    }

    void AddTexture(int ID)
    {
        uint textureID = RemapUtils.instance.GetTextureID(ID);
        if (textureID == 0) return;
        string iconPath = PluginHandlers.TextureProvider.GetIconPath(textureID)!;
        if (iconPath == null) return;
        IDalamudTextureWrap textureWrap = PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!;
        TextureWraps!.Add(textureWrap);
    }

    void DrawRightHeading(int column)
    {
        SetColumn(column);
        TransparentLabel(new Vector2(25, 0));
    }

    void DrawTexture(ref int textureIndex, Action drawExtra = null!)
    {
        nint texturePointer = nint.Zero;
        if (textureIndex < TextureWraps.Count)
            texturePointer = TextureWraps[textureIndex].ImGuiHandle;
        DrawTexture(texturePointer, drawExtra);
    }

    void DrawTexture(nint theint, Action drawExtraButton)
    {
        DrawTexture(theint);
        DrawRedownloadButton(drawExtraButton);
    }

    void DrawUserTexture(PettableUser u) => DrawTexture(u, () => DrawRedownloadButton(u));

    void DrawTexture(PettableUser u, Action drawExtraButton)
    {
        DrawTexture(ProfilePictureNetworked.instance.GetTexture(u));
        DrawRedownloadButton(drawExtraButton);
    }

    void DrawTexture(nint thenint)
    {
        TransparentLabel("", new Vector2(4, 0)); SameLineNoMargin();
        DrawImage(thenint, new Vector2(83, 83));
        SameLineNoMargin(); TransparentLabel("", new Vector2(4, 0));
    }

    public void SetOpenedAddPet(bool value)
    {
        searchField = string.Empty;
        foundNicknames.Clear();
        temporaryUser?.Reset();
        if (value == false && _openedAddPet != false)
        {
            user = PluginLink.PettableUserHandler.LocalUser()!;
            FillTextureList();
        }
        else if (value && !_openedAddPet && temporaryUser != null)
        {
            user = temporaryUser;
            FillTextureList();
        }
        _openedAddPet = value;
    }

    void FillTextureListFound()
    {
        DisposeTextures();
        AddTexture(-1);
    }

    protected override void OnDispose() => DisposeTextures();

    void DisposeTextures()
    {
        lastUser = user;
        foreach (IDalamudTextureWrap tWrap in TextureWraps)
            tWrap?.Dispose();
        TextureWraps?.Clear();
    }

    void DrawPetGraph(PettableUser forUser, Action drawExtra, Action<int, int> columnMid, Func<bool> skipFunc)
    {
        DrawGraph(new List<Action<int, int>>()
        {
            (row, column) => DrawTexture(ref texturePointer, drawExtra),
            columnMid.Invoke,
            (row, column) =>DrawRightHeading(column)
        }, () => graphPointer++ < forUser.SerializableUser.length, skipFunc);
    }

    void DrawGraph(List<Action<int, int>> onColumns, Func<bool> addRow, Func<bool> skipFunc = null!)
    {
        texturePointer = 0;
        graphPointer = -1;
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 01f));
        ImGui.BeginTable($"##Image Table{internalCounter++}", onColumns.Count, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY);
        DrawFillerBar(onColumns.Count);
        int column = 0;
        int row = 0;
        while (addRow?.Invoke() ?? false)
        {
            if (releaseGraph) break;
            column = 0;
            if (!(skipFunc != null && skipFunc.Invoke()))
            {
                foreach (Action<int, int> action in onColumns)
                {
                    SetColumn(column);
                    action?.Invoke(row, column);
                    column++;
                }
                ImGui.TableNextRow();
                DrawFillerBar(onColumns.Count);
                row++;
            }
            texturePointer++;
        }
        releaseGraph = false;
        ImGui.EndTable();
        ImGui.PopStyleVar();
    }


    public void Reset()
    {
        texturePointer = 0;
        graphPointer = -1;
        user = null!;
        lastUser = null!;
        temporaryUser = null!;
        existingUser = null!;
        removeMeUser = null!;
        youSureUser = null!;

        userMode = false;
        currentIsLocalUser = false;
        current = true;
        releaseGraph = false;
        _openedAddPet = false;
        youSureMode = false;
        advancedMode = false;
        existed = false;
        searchField = string.Empty;
        minionSearchField = string.Empty;

        foundNicknames.Clear();
        foreach (IDalamudTextureWrap tWrap in TextureWraps)
            tWrap?.Dispose();
        TextureWraps.Clear();

        importedData = null!;
    }
}
