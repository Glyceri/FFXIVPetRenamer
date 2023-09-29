using Dalamud.Interface.Internal;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Ipc.PenumbraIPCHelper;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Pet;
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
    readonly RenamablePet[] pets = new RenamablePet[2]
    {
        new RenamablePet(PetMode.Normal, "Minion"),
        new RenamablePet(PetMode.BattlePet, "Battle Pet")
    };

    PettableUser user = null!;
    RenamablePet activePet = null!;

    Vector2 baseSize = new Vector2(437, 192);
    Vector2 wideSize = new Vector2(335, 130);

    public PetRenameWindow() : base("Pet Nicknames", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize) { }

    public override void OnDraw()
    {
        Size = baseSize;
        user ??= PluginLink.PettableUserHandler.LocalUser()!;
        if (user == null) return;
        HandlePets();
    }

    internal override void OnPetModeChange(PetMode mode) => activePet = GetPet(mode);
    
    void HandlePets()
    {
        for (int i = 0; i < user.Pets.Length; i++)
        {
            PetBase pet = user.Pets[i];
            if (pets[i].petID == -1 && pet.Has)
            {
                pets[i].petID = pet.ID;
                pets[i].petName = pet.CustomName;
                pets[i].baseName = pet.BaseName;
                pets[i].temporaryPetName = pet.CustomName;
            }
            else if (pets[i].petID == -1) pets[i].Dispose();
        }
    }

    public override void OnLateDraw()
    {
        if (petMode == PetMode.ShareMode) return;
        if (activePet == null) return;
        if (activePet.textureWrap == null) return;

        SameLinePretendSpace();
        DrawImageBox();
    }

    public override void OnDrawNormal() => DrawInputField();
    public override void OnDrawBattlePet() => DrawInputField();

    void DrawInputField()
    {
        BeginListBox("##<stylingboxrenamepannel>", new Vector2(298, 119), StylingColours.titleBg);
        DrawInputFieldInsides();
        ImGui.EndListBox();
    }

    void DrawInputFieldInsides()
    {
        if (user == null) return;
        if (activePet == null) return;

        if (activePet.petID == -1 || activePet.baseName == string.Empty)
        {
            TextColoured(StylingColours.highlightText, $"Please summon a {activePet.referredToAs}.\nOr open the naming list: ");
            if (Button("Naming List")) PluginLink.WindowHandler.OpenWindow<PetListWindow>();
            SetTooltipHovered($"Opens the {activePet.referredToAs} List");
            return;
        }
        DrawPetNameField();
    }

    void DrawImageBox()
    {
        BeginListBox("##<stylingboxrenamepanne2l>", new Vector2(119, 119), StylingColours.titleBg);
        DrawImage(activePet.textureWrap.ImGuiHandle, new Vector2(111, 112));
        ImGui.EndListBox();
    }

    public override void OnDrawSharing()
    {
        Size = wideSize;
        PluginLink.WindowHandler.GetWindow<PetListWindow>()?.DrawExportHeader();
    }

    void DrawPetNameField()
    {
        string tempText = $"does not have a name!";
        if (activePet.petName.Length != 0) tempText = $"is named:";
        Label($"Your {activePet.baseName} {tempText}", Styling.PetWindowInsideBar, StylingColours.whiteText);
        SetTooltipHovered($"{activePet.baseName}");
        if (activePet.petName.Length != 0)
        {
            Label($"{activePet.petName}", Styling.PetWindowInsideBar, StylingColours.whiteText);
            SetTooltipHovered($"{activePet.petName}");
        }
        InputTextMultiLine(string.Empty, ref activePet._temporaryPetName, PluginConstants.ffxivNameSize, Styling.PetWindowInsideBar, ImGuiInputTextFlags.CtrlEnterForNewLine);
        SetTooltipHovered("Put in a nickname here.");
        DrawValidName();
    }

    void DrawValidName()
    {
        Button("Save Nickname", new Vector2(144, 25), "[Required to see a nickname]", Save); ImGui.SameLine(0, 1f);
        Button("Clear Nickname", new Vector2(144, 25), "[Clears the nickname from your list.]", Delete);
    }

    void Save()
    {
        user.SerializableUser.SaveNickname(activePet.petID, activePet.temporaryPetName, notifyICP: true);
        activePet.petName = activePet.temporaryPetName;
        OnButton();
    }

    void Delete()
    {
        user.SerializableUser.RemoveNickname(activePet.petID, notifyICP: true);
        OnButton();
    }

    void OnButton()
    {
        PluginLink.Configuration.Save();
        PenumbraIPCProvider.RedrawPetByIndex(activePet.petID);
    }

    public void OpenForId(int id, bool forceOpen = false)
    {
        if (forceOpen) ForceOpenForID(id);
        SetModeForID(id);
        if ((activePet = GetPet(petMode)) == null) return;
        if ((user ??= PluginLink.PettableUserHandler.LocalUser()!) == null) return;

        activePet.petID = id;
        activePet.baseName = RemapUtils.instance.PetIDToName(id).MakeTitleCase();
        activePet.petName = user.GetCustomName(id);
        activePet.temporaryPetName = activePet.petName;

        string iconPath = RemapUtils.instance.GetTextureID(id).GetIconPath();
        activePet.textureWrap = PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!;
    }

    void ForceOpenForID(int id)
    {
        if (id == -1) return;
        IsOpen = true;
        ImGui.SetNextWindowFocus();
    }

    void SetModeForID(int id)
    {
        if (id == -1) return;
        SetPetMode(id < -1 ? PetMode.BattlePet : PetMode.Normal);
    }

    RenamablePet GetPet(PetMode mode)
    {
        foreach (RenamablePet pet in pets)
            if (pet.associatedMode == mode) return pet;
        return null!;
    }

    protected override void OnDispose()
    {
        foreach (RenamablePet pet in pets)
            pet?.Dispose();
    }
}

internal class RenamablePet : IDisposable
{
    internal string petName = string.Empty;
    internal string _temporaryPetName = string.Empty;
    internal string temporaryPetName
    {
        get => (_temporaryPetName ?? string.Empty).Trim().Replace(PluginConstants.forbiddenCharacter.ToString(), "");
        set => _temporaryPetName = value ?? string.Empty;
    }
    internal string baseName = string.Empty;
    internal int petID = -1;
    internal IDalamudTextureWrap textureWrap = null!;

    internal PetMode associatedMode = PetMode.Normal;
    internal string referredToAs = string.Empty;

    internal RenamablePet(PetMode mode, string referredToAs)
    {
        associatedMode = mode;
        this.referredToAs = referredToAs;
    }

    public void Dispose()
    {
        textureWrap?.Dispose();
        textureWrap = null!;
    }
}
