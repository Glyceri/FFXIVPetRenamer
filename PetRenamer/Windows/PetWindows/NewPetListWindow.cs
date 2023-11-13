using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Sharing;
using PetRenamer.Core.Sharing.Importing.Data;
using PetRenamer.Core.Sharing.Importing;
using PetRenamer.Logging;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Numerics;
using System.Collections.Generic;
using System;
using Dalamud.Game.Text;
using Dalamud.Interface.Internal;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
[ModeTogglePetWindow]
internal class NewPetListWindow : PetWindow
{
    Vector2 baseSize = new Vector2(800, 500);
    Vector2 minSize = new Vector2(400, 240);

    const int HeaderHeight = 96;
    const int InnerHeaderHeight = 90;

    PettableUser activeUser = null!;
    List<DrawableElement> drawableElements = new List<DrawableElement>();

    SearchBarElement searchBarElement = new SearchBarElement();
    DrawableElement footerElement = null!;

    bool userlistActive = false;

    public NewPetListWindow() : base("Pet Nicknames List")
    {
        Size = baseSize;
        SizeCondition = ImGuiCond.FirstUseEver;
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = minSize,
            MaximumSize = new Vector2(9999, 9999)
        };
    }

    public override void OnDraw()
    {
        PettableUser localUser = PluginLink.PettableUserHandler.LocalUser()!;
        if ((activeUser ??= localUser!) == null) return;
        if (activeUser.UserChanged || activeUser.SerializableUser.changed || PluginLink.PettableUserHandler.Changed) HandleChanged();
        DrawHeader();
        DrawShareHeader();
    }

    public override void OnLateDraw()
    {
        if (drawableElements.Count == 0) return;
        if (!BeginListBox($"##<PetList{++internalCounter}>", new Vector2(ContentAvailableX, ContentAvailableY - (footerElement == null ? 0 : BarSizePadded + WindowPaddingY))))
            return;

        foreach (DrawableElement element in drawableElements)
            if (element.Draw(ref internalCounter, this))
                break;

        ImGui.EndListBox();
        footerElement?.Draw(ref internalCounter, this);
    }

    public override void OnWindowOpen() => OnPetModeChange(petMode);
    public override void OnWindowClose()
    {
        activeUser = null!;
        ClearList();
    }
    internal override void OnPetModeChange(PetMode mode) => HandleChanged();
    void HandleChanged()
    {
        if (activeUser == null) SetActiveToLocal();
        if (activeUser == null) return;

        if (!activeUser.Declared) searchBarElement.Clear();

        if((_lastMode == PetMode.ShareMode && petMode != PetMode.ShareMode) || (_lastMode != PetMode.ShareMode && petMode == PetMode.ShareMode))
        {
            SetActiveToLocal();
            ClearList();
            footerElement = null!;
        }

        if (petMode != PetMode.ShareMode)
        {
            if (!activeUser.Declared) SetActiveToLocal();
            HandleUserListActive();
            footerElement = null!;
        }
    }

    void SetActiveToLocal()
    {
        searchBarElement.Clear();
        activeUser = PluginLink.PettableUserHandler.LocalUser()!;
    }

    void DrawHeader()
    {
        if (activeUser == null) return;
        string activeUsername = StringUtils.instance.MakeTitleCase(activeUser.UserName);
        string homeWorldName = SheetUtils.instance.GetWorldName(activeUser.Homeworld);
        bool ipcUser = activeUser.IsIPCOnlyUser;
        int accuratePetCount = activeUser.SerializableUser.AccurateTotalPetCount();
        int accurateMinionCount = activeUser.SerializableUser.AccurateMinionCount();
        int accurateBattlePetCount = activeUser.SerializableUser.AccurateBattlePetCount();
        if (!BeginListBox($"##<PetList{internalCounter++}>", new Vector2(250, HeaderHeight)))
            return;

        DrawUserTextureEncased(activeUser);
        SameLinePretendSpace();

        if (BeginListBoxAutomatic($"##<PetList{internalCounter++}>", new Vector2(ContentAvailableX, ContentAvailableY), ipcUser))
        {
            if (petMode == PetMode.ShareMode) Label($"{activeUsername}", new Vector2(ContentAvailableX, BarSize), $"Username: {activeUsername}");
            else if (Button($"{activeUsername}", new Vector2(ContentAvailableX, BarSize), $"[Change User]\nCurrent User: {activeUsername}"))
                ToggleUserList(true);

            OverrideLabel($"{homeWorldName}", new Vector2(ContentAvailableX, BarSize), $"Homeworld: {homeWorldName}");

            if (ipcUser) OverrideLabel($"[User added via IPC]", new Vector2(ContentAvailableX, BarSize), "This user is added via IPC, it will not get saved automatically.");
            else OverrideLabel($"[{accuratePetCount}, {accurateMinionCount}, {accurateBattlePetCount}]", new Vector2(ContentAvailableX, BarSize), $"Total Pet Count: {accuratePetCount}, Minion Count: {accurateMinionCount}, Battle Pet Count: {accurateBattlePetCount}");

            ImGui.EndListBox();
        }

        ImGui.EndListBox();
    }

    void ToggleUserList(bool reset = false)
    {
        userlistActive = !userlistActive;
        if (!userlistActive && reset) SetActiveToLocal();
        HandleUserListActive();
    }

    void HandleUserListActive()
    {
        ClearList();
        if (userlistActive) FillUserList();
        else FillMinionOrPetList();
    }

    void FillUserList()
    {
        drawableElements.Add(new WarningDrawElement("Help! I cannot see any profile pictures. Please enable: [Allow Automatic Profile Pictures] in the settings menu.", PluginLink.WindowHandler.OpenWindow<ConfigWindow>, SeIconChar.MouseWheel.ToIconString(), "Open settings menu!"));
        PluginLink.PettableUserHandler.LoopThroughUsers(u =>
        {
            PlayerDrawElement p = new PlayerDrawElement(u);
            if (u.LocalUser) 
            { 
                drawableElements.Insert(1, p);
                drawableElements.Insert(2, new InvisibleDrawElement());
            }
            else drawableElements.Add(p);
        });
    }
    void FillMinionOrPetList()
    {
        if (activeUser == null) return;

        string identifier = string.Empty;
        Func<int, bool> isValid = null!;
        Action<int> callback = (id) => { };
        Action<int, bool> callback2 = null!;
        if (activeUser.LocalUser)
        {
            callback2 = (id, ipc) =>
            {
                activeUser.SerializableUser.SaveNickname(id, string.Empty, true, false, ipc);
                if (!ipc) IpcUtils.instance.NotifyChange(id, string.Empty);
            };
        }

        if (petMode == PetMode.Normal)
        {
            if(activeUser.LocalUser) drawableElements.Add(searchBarElement);
            identifier = "Minion";
            isValid = (id) => id > -1;
            callback = (id) => PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForMinion(id, true);
        }
        else
        {
            drawableElements.Add(new WarningDrawElement("Petglamour is now fully compatible!"));
            identifier = "Battle Pet";
            isValid = (id) => id < -1;
            callback = (id) => PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForBattlePet(id, true);
        }

        for (int i = 0; i < activeUser.SerializableUser.length; i++)
        {
            QuickName name = activeUser.SerializableUser[i];
            if (!isValid(name.ID)) continue;
            string petBaseName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(name.ID));
            IDalamudTextureWrap textureWrap = GetTexture(name.ID);
            bool isIPC = name.HasIPCName;
            Action<int> internalCallback = (id) => { };
            if (activeUser.LocalUser) internalCallback = callback;
            drawableElements.Add(new DrawablePet(isIPC, textureWrap, identifier, name.Name, petBaseName, name.ID, internalCallback, "X", isIPC ? "Clear IPC" : "Remove", callback2));
        }
    }

    IDalamudTextureWrap GetTexture(int id)
    {
        uint textureID = RemapUtils.instance.GetTextureID(id);
        if (textureID == 0) return null!;
        string iconPath = PluginHandlers.TextureProvider.GetIconPath(textureID)!;
        if (iconPath == null) return null!;
        return PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!;
    }

    public void DrawShareHeader()
    {
        SameLinePretendSpace2();

        if (!BeginListBox($"##<PetList{internalCounter++}>", new Vector2(ContentAvailableX, HeaderHeight)))
            return;

        if (BeginListBox($"##<PetList{internalCounter++}>", new Vector2(ContentAvailableX, ContentAvailableY)))
        {
            OverrideLabel("You can export all your pet names to your clipboard and send those to a friend.", new Vector2(ContentAvailableX, BarSize));
            OverrideLabel("A friend can import your code to see your names.", new Vector2(ContentAvailableX, BarSize));

            Button($"Export to Clipboard##clipboardExport{internalCounter++}", new Vector2(ContentAvailableX / 2, BarSize), PluginConstants.Strings.exportTooltip, ExportClipboard);
            SameLine();
            Button($"Import from Clipboard##clipboardImport{internalCounter++}", new Vector2(ContentAvailableX, BarSize), PluginConstants.Strings.importTooltip, ImportClipboard);

            ImGui.EndListBox();
        }

        ImGui.EndListBox();
    }

    void ExportClipboard()
    {
        if (ImGui.IsKeyDown(ImGuiKey.LeftShift) || PluginLink.Configuration.alwaysOpenAdvancedMode) FillExportList();
        else
        {
            if(petMode == PetMode.ShareMode) ClearList();
            SetActiveToLocal();
            SharingHandler.SetupList(activeUser);
            DoExport();
        }
    }

    void FillExportList()
    {
        IsOpen = true;
        SetPetMode(PetMode.ShareMode);
        SetActiveToLocal();
        if (activeUser == null) return;
        ClearList();

        SharingHandler.SetupList(activeUser);

        for (int i = 0; i < activeUser.SerializableUser.length; i++)
        {
            QuickName nickname = activeUser.SerializableUser[i];
            if (nickname.ID == -1) continue;
            if (nickname.RawName == string.Empty) continue;

            string petBaseName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.ID));
            IDalamudTextureWrap textureWrap = GetTexture(nickname.ID);
            SharingHandler.SetContainsList(i, nickname.Name != string.Empty);
            drawableElements.Add(new DrawablePet(false, textureWrap, string.Empty, nickname.RawName, petBaseName, nickname.ID, (id) => { }, string.Empty, string.Empty, null!, i, (index) =>
            {
                bool value = SharingHandler.GetContainsList(index);
                Checkbox(string.Empty + $"##<checker>{++internalCounter}", "Include pet in export", ref value);
                SharingHandler.SetContainsList(index, value);
            }));
        }

        footerElement = new ButtonElement("Export", "Export Nicknames List", DoExport);
    }
    void DoExport()
    {
        if (petMode == PetMode.ShareMode) ClearList();
        SharingHandler.Export(activeUser);
        footerElement = null!;
    }

    void FillImportList(SucceededImportData data)
    {
        for(int i = 0; i < data.ids.Length; i++)
        {
            string petBaseName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(data.ids[i]));
            IDalamudTextureWrap textureWrap = GetTexture(data.ids[i]);
            drawableElements.Add(new DrawablePet(false, textureWrap, string.Empty, data.names[i], petBaseName, data.ids[i], (id) => { }, string.Empty, string.Empty, null!, i, (index) => Label(data.GetString(data.importTypes[index]), Styling.SmallButton)));
        }
        footerElement = new ButtonElement("Import", "Import Nicknames List", () => DoImport(data));
    }

    void ImportClipboard()
    {
        ImportData data = ImportHandler.SetImportString(ImGui.GetClipboardText());
        if (data is FailedImportData failedImportData) PetLog.Log($"Import Error occured: {failedImportData.ErrorMessage}");
        if (data is SucceededImportData succeededImportData)
        {
            IsOpen = true;
            SetPetMode(PetMode.ShareMode);
            PluginLink.PettableUserHandler.DeclareUser(new SerializableUserV3(succeededImportData.UserName, succeededImportData.HomeWorld), Core.PettableUserSystem.Enums.UserDeclareType.Add);
            activeUser = PluginLink.PettableUserHandler.GetUser(succeededImportData.UserName, succeededImportData.HomeWorld)!;
            FillImportList(succeededImportData);
        }
    }

    void DoImport(SucceededImportData data)
    {
        for (int i = 0; i < data.ids.Length; i++)
        {
            if (data.importTypes[i] == ImportType.Remove) activeUser.SerializableUser.RemoveNickname(data.ids[i]);
            else activeUser.SerializableUser.SaveNickname(data.ids[i], data.names[i], data.importTypes[i] == ImportType.New || data.importTypes[i] == ImportType.Rename || i == data.ids.Length - 1, false);
        }
        PluginLink.Configuration.Save();
        activeUser = PluginLink.PettableUserHandler.GetUser(data.UserName, data.HomeWorld)!;
        if (activeUser.LocalUser) IpcUtils.instance.SendAllData();
        if (petMode == PetMode.ShareMode) ClearList();
        footerElement = null!;
    }

    void ClearList()
    {
        foreach (DrawableElement element in drawableElements)
            element?.Dispose();
        drawableElements.Clear();
    }

    class DrawableElement : IDisposable
    {
        public void Dispose() => OnDispose();
        public virtual void OnDispose() { }
        public virtual bool Draw(ref int internalcounter, NewPetListWindow window) => false;
    }


    class ButtonElement : DrawableElement 
    {
        string text;
        string tooltip;
        Action callback;

        public ButtonElement(string text, string tooltip, Action callback)
        {
            this.text = text;
            this.tooltip = tooltip;
            this.callback = callback;
        }

        public override bool Draw(ref int internalcounter, NewPetListWindow window)
        {
            if (!window.BeginListBoxSub($"##<we>{++internalcounter}", new Vector2(ContentAvailableX, BarSizePadded))) return false;
            window.Button(text, ContentAvailable, tooltip, callback);
            ImGui.EndListBox();
            return false;
        }
    }

    class WarningDrawElement : DrawableElement
    {
        string warning;
        Action callback;
        string callbackText;
        string callbackTooltip;
        public WarningDrawElement(string warning, Action callback = null!, string callbackText = "", string callbackTooltip = "")
        {
            this.warning = warning;
            this.callback = callback;
            this.callbackText = callbackText;
            this.callbackTooltip = callbackTooltip;
        }

        public override bool Draw(ref int internalcounter, NewPetListWindow window)
        {
            if (!window.BeginListBoxSub($"##<we>{++internalcounter}", new Vector2(ContentAvailableX, BarSizePadded))) return false;
            window.DrawWarningLabel(warning, callbackText, callbackTooltip, callback);
            ImGui.EndListBox();
            return false;
        }
    }

    class DrawablePet : DrawableElement
    {
        IDalamudTextureWrap texture;
        string identifier;
        string customName;
        string baseName;
        int baseId;

        string buttonText;
        string buttonTooltip;
        Action intCallback = null!;
        Action<int> callback2;
        Action internalCallback = null!;

        bool isIpc;

        public DrawablePet(bool isIpc, IDalamudTextureWrap texture, string identifier, string customName, string baseName, int baseId, Action<int> callback2, string buttonText, string buttonTooltip, Action<int, bool> callback, int index = 0, Action<int> drawExtraButton = null!)
        {
            this.isIpc = isIpc;
            this.identifier = identifier;
            this.texture = texture;
            this.customName = customName;
            this.baseName = baseName;
            this.baseId = baseId;

            this.buttonText = buttonText;
            this.buttonTooltip = buttonTooltip;
            if (callback != null) intCallback = () => callback(baseId, isIpc);
            this.callback2 = callback2;
            if(drawExtraButton != null) internalCallback = () => drawExtraButton(index);
        }

        public override void OnDispose() => texture?.Dispose();

        public override bool Draw(ref int internalcounter, NewPetListWindow window)
        {
            if (texture == null) return false;
            if (!window.BeginListBoxAutomaticSub($"##<we>{++internalcounter}", new Vector2(ContentAvailableX, HeaderHeight), isIpc)) return false;

            if (window.BeginListBoxAutomatic($"##<PetList{internalCounter++}>", new Vector2(91, 90), isIpc))
            {
                window.DrawTexture(texture.ImGuiHandle, internalCallback);
                ImGui.EndListBox();
            }

            window.SameLinePretendSpace();
            if (window.BeginListBoxAutomaticSub($"##<we>{++internalcounter}", new Vector2(ContentAvailableX, InnerHeaderHeight), isIpc))
            {
                window.DrawAdvancedBarWithQuit($"Nickname", customName, () => callback2(baseId), buttonText, $"{buttonTooltip} {baseName}", intCallback);
                window.DrawBasicBar($"{identifier} Name", baseName);
                window.DrawBasicBar($"{identifier} ID", baseId.ToString());
                ImGui.EndListBox();
            }
            ImGui.EndListBox();
            return false;
        }
    }

    class SearchBarElement : DrawableElement
    {
        string searchText = string.Empty;
        string lastText = string.Empty;

        public override bool Draw(ref int internalcounter, NewPetListWindow window)
        {
            bool outcome = false;
            if (!window.BeginListBoxSub($"##<we>{++internalcounter}", new Vector2(ContentAvailableX, BarSizePadded))) return false;
            window.InputTextMultiLine(string.Empty, ref searchText, PluginConstants.ffxivNameSize, new Vector2(ContentAvailableX - window.Styling.SmallButton.X - FramePaddingX - SpaceSize, BarSize), ImGuiInputTextFlags.CtrlEnterForNewLine, $"Search through all Minions in the game and add them for nicknaming.\n(Search possible on Name and ID)");
            window.SameLinePretendSpace();
            window.XButton("X", window.Styling.SmallButton, "Clear search field", () => searchText = string.Empty);
            if (searchText != lastText)
            {
                lastText = searchText;
                window.ClearList();
                if (searchText != string.Empty)
                {
                    List<SerializableNickname> nicknames = SheetUtils.instance.GetThoseThatContain(searchText);
                    window.activeUser = new PettableUser($"[{searchText}]", 9999, new SerializableUserV3(new int[0], new string[0], searchText, 9999, PluginConstants.baseSkeletons, PluginConstants.baseSkeletons));
                    window.drawableElements.Add(this);
                    foreach(SerializableNickname nickname in nicknames)
                    {
                        window.drawableElements.Add(
                            new DrawablePet(false,
                            window.GetTexture(nickname.ID), 
                            "Minion", 
                            nickname.Name, 
                            StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(nickname.ID)), 
                            nickname.ID, 
                            (id) => PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForMinion(id, true),
                            $"+##{++internalcounter}", $"Add Minion", (id, ipc) => 
                            {
                                Clear();
                                PluginLink.WindowHandler.GetWindow<PetRenameWindow>().OpenForMinion(id, true);
                            }));
                    }
                }
                else
                {
                    window.SetActiveToLocal();
                    window.FillMinionOrPetList();
                }
                outcome = true;
            }

            ImGui.EndListBox();
            return outcome;
        }

        public void Clear() => searchText = string.Empty;
    }

    class InvisibleDrawElement : DrawableElement
    {
        public override bool Draw(ref int internalcounter, NewPetListWindow window)
        {
            if (!window.BeginListBoxAutomatic($"##<we>{++internalcounter}", new Vector2(ContentAvailableX, BarSize), false)) return false;

            ImGui.EndListBox();
            return false;
        }
    }

    class PlayerDrawElement : DrawableElement
    {
        PettableUser myUser;
        public PlayerDrawElement(PettableUser user) => myUser = user;

        bool sureMode = false;

        public override bool Draw(ref int internalcounter, NewPetListWindow window)
        {
            bool outcome = false;
            if (!window.BeginListBoxAutomaticSub($"##<we>{++internalcounter}", new Vector2(ContentAvailableX, HeaderHeight), myUser.IsIPCOnlyUser)) return false;
            window.DrawUserTextureEncased(myUser);
            window.SameLinePretendSpace();
            if (window.BeginListBoxAutomatic($"##<we>{++internalcounter}", new Vector2(ContentAvailableX, InnerHeaderHeight), myUser.IsIPCOnlyUser))
            {
                window.DrawAdvancedBarWithQuit($"User Name", myUser.UserName, () =>
                {
                    outcome = true;
                    window.activeUser = myUser;
                    window.searchBarElement.Clear();
                    window.ToggleUserList();
                }, "X", $"Remove User: {myUser.UserName} @ {myUser.HomeWorldName}", () => sureMode = !sureMode);
                if (!sureMode)
                {
                    window.DrawBasicBar($"Homeworld", myUser.HomeWorldName);
                    window.DrawBasicBar($"Petcount", $"Total: {myUser.SerializableUser.AccurateTotalPetCount()}, Minions: {myUser.SerializableUser.AccurateMinionCount()}, Battle Pets: {myUser.SerializableUser.AccurateBattlePetCount()}");
                }
                else
                {
                    if (myUser.LocalUser) sureMode = false;
                    window.DrawYesNoBar($"Are you sure you want to delete {myUser.UserName} @ {myUser.HomeWorldName}##{++internalcounter}", myUser.Destroy, () => sureMode = false);
                }
                ImGui.EndListBox();
            }
            ImGui.EndListBox();
            return outcome;
        }
    }
}