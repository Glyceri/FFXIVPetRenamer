using Dalamud.Interface.Internal;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Ipc.PenumbraIPCHelper;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Numerics;

namespace PetRenamer.Windows.PetWindows;

[MainPetWindow]
[PersistentPetWindow]
[ModeTogglePetWindow]
public class PetRenameWindow : InitializablePetWindow
{
    string companionName = string.Empty;
    string battlePetName = string.Empty;

    string temporaryCompanionName = string.Empty;
    string temporaryBattlePetName = string.Empty;

    string companionBaseName = string.Empty;
    string battlePetBaseName = string.Empty;

    int companionID = -1;
    int battlePetID = -1;

    Vector2 baseSize = new Vector2(437, 188);
    Vector2 bPetSize = new Vector2(437, 188);
    Vector2 wideSize = new Vector2(335, 127);

    IDalamudTextureWrap textureWrap = null!;
    IDalamudTextureWrap textureWrapPet = null!;

    public PetRenameWindow() : base("Pet Nicknames", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse)
    {
        Size = baseSize;
    }

    PettableUser user = null!;

    public override void OnDraw()
    {
        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;

        if (companionID == -1 && user.Minion.Has)
        {
            companionID = user.Minion.ID;
            companionName = user.Minion.CustomName;
            companionBaseName = user.Minion.BaseName;
            temporaryCompanionName = companionName;
        }
        else if (companionID == -1)
        {
            textureWrap?.Dispose();
            textureWrap = null!;
        }

        if (battlePetID == -1 && user.BattlePet.Has)
        {
            battlePetID = user.BattlePet.ID;
            battlePetBaseName = user.BattlePet.BaseName;
            battlePetName = user.BattlePet.CustomName;
            temporaryBattlePetName = battlePetName;
        }
        else if (battlePetID == -1)
        {
            textureWrapPet?.Dispose();
            textureWrapPet = null!;
        }

        if (petMode != PetMode.ShareMode)
        BeginListBox("##<stylingboxrenamepannel>", new Vector2(298, 119), StylingColours.titleBg);
    }

    public override void OnLateDraw()
    {
        if (petMode != PetMode.ShareMode) 
        { 
            ImGui.EndListBox();

            if (petMode == PetMode.Normal && textureWrap == null) return;
            if (petMode == PetMode.BattlePet && textureWrapPet == null) return;
            SameLinePretendSpace();
            BeginListBox("##<stylingboxrenamepanne2l>", new Vector2(119, 119), StylingColours.titleBg);
            if(PluginLink.Configuration.displayImages)
            ImGui.Image(petMode == PetMode.Normal ? textureWrap.ImGuiHandle : textureWrapPet.ImGuiHandle, new Vector2(111, 112));
            else
            {
                PushStyleColor(ImGuiCol.Button, StylingColours.defaultBackground);
                PushStyleColor(ImGuiCol.ButtonActive, StylingColours.defaultBackground);
                PushStyleColor(ImGuiCol.ButtonHovered, StylingColours.defaultBackground);
                ImGui.Button("", new Vector2(111, 112));
            } 
                
            ImGui.EndListBox();
        }
    }

    public override void OnDrawNormal()
    {
        Size = baseSize;
        if (user == null) return;
        if (companionID == -1)
        {
            TextColoured(StylingColours.highlightText, $"Please summon a minion.\nOr open the naming list: ");
            if (Button("Naming List")) PluginLink.WindowHandler.OpenWindow<PetListWindow>();
            SetTooltipHovered("Opens the Minion List");
        }
        else DrawPetNameField(companionBaseName, ref companionName, ref temporaryCompanionName, ref companionID);
    }

    public override void OnDrawBattlePet()
    {
        Size = bPetSize;
        if (user == null) return;
        if (battlePetID == -1)
        {
            if (battlePetBaseName == string.Empty)
            {
                TextColoured(StylingColours.highlightText, $"Please summon a Battle Pet.\nOr open the naming list: ");
                if (Button("Naming List")) PluginLink.WindowHandler.OpenWindow<PetListWindow>();
                SetTooltipHovered("Opens the Battle Pet List");
            }
            else TextColoured(StylingColours.highlightText, $"Please summon your {battlePetBaseName}.");
        }
        else DrawPetNameField(battlePetBaseName, ref battlePetName, ref temporaryBattlePetName, ref battlePetID);
    }

    public override void OnDrawSharing()
    {
        Size = wideSize;
        PluginLink.WindowHandler.GetWindow<PetListWindow>()?.DrawExportHeader();
    }

    void DrawPetNameField(string basePet, ref string temporaryName, ref string temporaryCustomName, ref int theID)
    {
        string tempText = $"does not have a name!";
        if (temporaryName.Length != 0) tempText = $"is named:";
        Label($"Your {basePet} {tempText}", new Vector2(290, 25),StylingColours.whiteText);
        if (basePet != string.Empty) SetTooltipHovered($"{basePet}");
        if (temporaryName.Length != 0)
        {
            Label($"{temporaryName}", new Vector2(290, 25), StylingColours.whiteText);
            SetTooltipHovered($"{temporaryName}");
        }
        temporaryCustomName ??= string.Empty;
        InputTextMultiLine(string.Empty, ref temporaryCustomName, PluginConstants.ffxivNameSize, new Vector2(290, 25), ImGuiInputTextFlags.CtrlEnterForNewLine);
        temporaryCustomName ??= string.Empty;
        SetTooltipHovered("Put in a nickname here.");
        temporaryCustomName = temporaryCustomName.Trim();
        DrawValidName(temporaryCustomName, ref theID);
    }

    void DrawValidName(string internalTempText, ref int theID)
    {
        if (Button("Save Nickname", new Vector2(144, 25), "[Required to see a nickname]"))
        {
            user.SerializableUser.SaveNickname(theID, internalTempText.Replace(PluginConstants.forbiddenCharacter.ToString(), ""), notifyICP: true);
            PluginLink.Configuration.Save();
            if (companionID == theID)
            {
                companionName = internalTempText;
                PenumbraIPCProvider.RedrawMinionByIndex(user.Minion.Index);
            }
            if (battlePetID == theID)
            {
                battlePetName = internalTempText;
                PenumbraIPCProvider.RedrawBattlePetByIndex(user.BattlePet.Index);
            }
        }
        ImGui.SameLine(0, 1f);
        if (Button("Clear Nickname", new Vector2(144, 25), "Clears the nickname from your list."))
        {
            user.SerializableUser.RemoveNickname(theID, notifyICP: true);
            PluginLink.Configuration.Save();
            if (companionID == theID)
            {
                companionID = -1;
                PenumbraIPCProvider.RedrawMinionByIndex(user.Minion.Index);
            }
            if (battlePetID == theID)
            {
                battlePetID = -1;
                PenumbraIPCProvider.RedrawBattlePetByIndex(user.BattlePet.Index);
            }
        }
    }

    public void OpenForId(int id, bool forceOpen = false)
    {
        if (forceOpen) { IsOpen = true; ImGui.SetWindowFocus(); }
        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        companionID = id;
        companionBaseName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(companionID));
        companionName = user.GetCustomName(companionID);
        temporaryCompanionName = companionName;

        if (companionID == -1) return;
        string iconPath = PluginHandlers.TextureProvider.GetIconPath(RemapUtils.instance.GetTextureID(companionID))!;
        if (iconPath == null) return;
        textureWrap = PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!;
    }

    public void OpenForBattleID(int id, bool forceOpen = false)
    {
        if (forceOpen) { IsOpen = true; ImGui.SetWindowFocus(); }
        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        battlePetID = id;
        battlePetBaseName = RemapUtils.instance.PetIDToName(id);
        battlePetName = user.GetCustomName(battlePetID);
        temporaryBattlePetName = battlePetName;

        if (battlePetID == -1) return;
        string iconPath = PluginHandlers.TextureProvider.GetIconPath(RemapUtils.instance.GetTextureID(battlePetID))!;
        if (iconPath == null) return;
        textureWrapPet = PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!;
    }

    public override void OnInitialized()
    {

    }

    protected override void OnDispose()
    {
        textureWrap?.Dispose();
        textureWrapPet?.Dispose();
    }
}
