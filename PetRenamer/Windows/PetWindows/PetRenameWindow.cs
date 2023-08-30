using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
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

    Vector2 baseSize = new Vector2(310, 158);
    Vector2 wideSize = new Vector2(335, 127);

    public PetRenameWindow() : base("Pet Nickname", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse)
    {
        Size = baseSize;
    }

    PettableUser user = null!;

    public override void OnDraw()
    {
        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;

        if (companionID == -1 && user.HasCompanion)
        {
            companionID = user.CompanionID;
            companionName = user.CustomCompanionName;
            companionBaseName = user.CompanionBaseName;
            temporaryCompanionName = companionName;
        }

        if (battlePetID == -1 && user.HasBattlePet)
        {
            battlePetID = user.BattlePetID;
            battlePetBaseName = user.BaseBattelPetName;
            battlePetName = user.BattlePetCustomName;
            temporaryBattlePetName = battlePetName;
        }
    }

    public override void OnDrawNormal()
    {
        Size = baseSize;
        if (user == null) return;
        if (!user.HasCompanion) ImGui.TextColored(StylingColours.highlightText, "Please summon a minion.");
        else DrawPetNameField(companionBaseName, ref companionName, ref temporaryCompanionName, ref companionID);
    }

    public override void OnDrawBattlePet()
    {
        Size = baseSize;
        if (user == null) return;
        if (!user.HasBattlePet)
        {
            if (battlePetBaseName == string.Empty) ImGui.TextColored(StylingColours.highlightText, $"Please summon a Battle Pet.");
            else ImGui.TextColored(StylingColours.highlightText, $"Please summon your {battlePetBaseName}.");
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
        ImGui.TextColored(StylingColours.whiteText, $"Your");               SameLinePretendSpace();
        ImGui.TextColored(StylingColours.highlightText, $"{basePet}");      SameLinePretendSpace();
        if (ImGui.IsItemHovered()) ImGui.SetTooltip($"{basePet}");
        if (temporaryName.Length == 0) ImGui.TextColored(StylingColours.whiteText, $"does not have a name!");
        else
        {
            ImGui.TextColored(StylingColours.whiteText, $"is named:");
            if (temporaryName.Length < 10) SameLinePretendSpace();
            ImGui.TextColored(StylingColours.highlightText, $"{temporaryName}");
            if (ImGui.IsItemHovered()) ImGui.SetTooltip($"{temporaryName}");
        }
        InputText(string.Empty, ref temporaryCustomName, PluginConstants.ffxivNameSize);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Put in a nickname here.");
        temporaryCustomName = temporaryCustomName.Trim();
        DrawValidName(temporaryCustomName, ref theID);
    }

    void DrawValidName(string internalTempText, ref int theID)
    {
        if (Button("Save Nickname")) 
        { 
            user.SerializableUser.SaveNickname(theID, internalTempText.Replace("^", ""), notifyICP: true); 
            PluginLink.Configuration.Save(); 
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("[Required to see a nickname]");
        ImGui.SameLine(0, 1f);
        if (Button("Remove Nickname"))
        {
            user.SerializableUser.RemoveNickname(theID, notifyICP: true); 
            PluginLink.Configuration.Save();
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Removes the nickname from your list.");
    }

    public void OpenForId(int id, bool forceOpen = false)
    {
        if (forceOpen) IsOpen = true;
        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        companionID = id;
        companionBaseName = StringUtils.instance.MakeTitleCase(SheetUtils.instance.GetPetName(companionID));
        companionName = user.GetCustomName(companionID);
        temporaryCompanionName = companionName;
        
    }

    public void OpenForBattleID(int id, bool forceOpen = false)
    {
        if (forceOpen) IsOpen = true;
        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        battlePetID = id;
        battlePetBaseName = RemapUtils.instance.PetIDToName(id);
        battlePetName = user.GetCustomName(battlePetID);
        temporaryBattlePetName = battlePetName;
    }

    public override void OnInitialized()
    {

    }
}
