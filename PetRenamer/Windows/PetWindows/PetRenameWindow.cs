using Dalamud.Interface.Internal;
using Dalamud.Logging;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Ipc.PenumbraIPCHelper;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System;
using System.Numerics;

namespace PetRenamer.Windows.PetWindows;

[MainPetWindow]
[PersistentPetWindow]
[ModeTogglePetWindow]
public class PetRenameWindow : PetWindow
{
    PettableUser user = null!;

    PetData companionData = new PetData();
    PetData battlePetData = new PetData();

    PetData activeData = null!;

    Vector2 baseSize = new Vector2(437, 188);
    Vector2 bPetSize = new Vector2(437, 188);
    Vector2 wideSize = new Vector2(335, 127);

    public PetRenameWindow() : base("Pet Nicknames") => Size = baseSize;

    public override void OnDraw()
    {
        if (petMode == PetMode.ShareMode) return;
        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;

        if (companionData.id == -1 && user.Minion.Has) Set(ref companionData, user.Minion.ID);
        else if (companionData.id == -1) companionData?.Dispose();

        if (battlePetData.id == -1 && user.BattlePet.Has) Set(ref battlePetData, user.Minion.ID);
        else if (battlePetData.id == -1) battlePetData?.Dispose();

        activeData = companionData!;
        if (petMode == PetMode.BattlePet) activeData = battlePetData!;
        activeData.Reset();

        BeginListBox("##<stylingboxrenamepannel>", new Vector2(298, 119), StylingColours.titleBg);
        if (petMode == PetMode.Normal) HandleNamingField("Minion");
        if (petMode == PetMode.BattlePet) HandleNamingField("Battle Pet");
        ImGui.EndListBox();
        SameLinePretendSpace();
    }

    internal override void OnPetModeChange(PetMode mode) => activeData?.FullReset();
    public override void OnDrawNormal() => Size = baseSize;
    public override void OnDrawBattlePet() => Size = bPetSize;
    public override void OnDrawSharing()
    {
        Size = wideSize;
        PluginLink.WindowHandler.GetWindow<PetListWindow>()?.DrawExportHeader();
    }

    public override void OnLateDraw()
    {
        if (petMode == PetMode.ShareMode) return;
        if (activeData.textureWrap == null) return;

        BeginListBox("##<stylingboxrenamepanne2l>", new Vector2(119, 119), StylingColours.titleBg);
        Image(activeData.textureWrap.ImGuiHandle, new Vector2(111, 112));
        ImGui.EndListBox();
    }

    public void HandleNamingField(string petTypeIdentifier)
    {
        if (activeData.id == -1) DrawNoPetField(petTypeIdentifier);
        else DrawPetNameField();
    }

    void DrawNoPetField(string petTypeIdentifier)
    {
        TextColoured(StylingColours.highlightText, $"Please summon a {petTypeIdentifier}.\nOr open the naming list: ");
        if (Button("Naming List")) PluginLink.WindowHandler.OpenWindow<PetListWindow>();
        SetTooltipHovered($"Opens the {petTypeIdentifier} List");
    }

    void DrawPetNameField()
    {
        string tempText = $"does not have a name!";
        if (activeData.customName.Length != 0) tempText = $"is named:";
        Label($"Your {activeData.baseName} {tempText}", new Vector2(290, 25), StylingColours.whiteText);
        if (activeData.baseName != string.Empty) SetTooltipHovered($"{activeData.baseName}");
        if (activeData.customName.Length != 0)
        {
            Label($"{activeData.customName}", new Vector2(290, 25), StylingColours.whiteText);
            SetTooltipHovered($"{activeData.customName}");
        }

        InputTextMultiLine(string.Empty, ref activeData.temporaryName, PluginConstants.ffxivNameSize, new Vector2(290, 25), ImGuiInputTextFlags.CtrlEnterForNewLine);
        SetTooltipHovered("Put in a nickname here.");
        activeData.temporaryName = activeData.temporaryName.Trim();
        DrawValidName();
    }

    void DrawValidName()
    {
        if (Button("Save Nickname", new Vector2(144, 25), "[Required to see a nickname]"))
        {
            user.SerializableUser.SaveNickname(activeData.id, activeData.temporaryName.Replace(PluginConstants.forbiddenCharacter, ""), notifyICP: true);
            PenumbraIPCProvider.RedrawByID(activeData.id);
            PluginLink.Configuration.Save();
        }
        ImGui.SameLine(0, 1f);
        if (Button("Clear Nickname", new Vector2(144, 25), "Clears the nickname from your list."))
        {
            user.SerializableUser.RemoveNickname(activeData.id, notifyICP: true);
            PenumbraIPCProvider.RedrawByID(activeData.id);
            PluginLink.Configuration.Save();
            activeData.id = -1;
        }
    }

    public void OpenForId(int id, bool forceOpen = false)
    {
        if (forceOpen) 
        { 
            IsOpen = true; 
            ImGui.SetNextWindowFocus();
            if (id < 8000) SetPetMode(PetMode.BattlePet);
            else SetPetMode(PetMode.Normal);
        }

        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        if (id == -1) return;
        if (id < 8000) Set(ref battlePetData, id);
        if (id > 8000) Set(ref companionData, id);
    }

    void Set(ref PetData pData, int id) 
    {
        pData.id = id;
        pData.baseName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(id));
        pData.customName = user.GetCustomName(id);
        pData.temporaryName = pData.customName;
        SetTextureWrap(id, ref pData.textureWrap);
    }

    void SetTextureWrap(int id, ref IDalamudTextureWrap textureWrap)
    {
        if (id == -1) return;
        string iconPath = PluginHandlers.TextureProvider.GetIconPath(RemapUtils.instance.GetTextureID(id))!;
        if (iconPath == null) return;
        textureWrap = PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!;
    }

    protected override void OnDispose()
    {
        companionData?.textureWrap?.Dispose();
        battlePetData?.textureWrap?.Dispose();
    }

    class PetData : IDisposable
    {
        public int id = -1;
        public string baseName = string.Empty;
        public string customName = string.Empty;
        public string temporaryName = string.Empty;
        public IDalamudTextureWrap textureWrap = null!;

        public void FullReset()
        {
            id = -1;
            Reset();
        }

        public void Reset()
        {
            baseName ??= string.Empty;
            customName ??= string.Empty;
            temporaryName ??= string.Empty;
        }

        public void Dispose()
        {
            textureWrap?.Dispose();
            textureWrap = null!;
        }
    }
}
