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

    Vector2 minSize = new Vector2(220, 192);
    Vector2 baseSize = new Vector2(437, 192);
    Vector2 wideSize = new Vector2(1500, 192);

    Vector2 imageBoxSize = new Vector2(119, 119);

    public PetRenameWindow() : base("Pet Nicknames", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse) { }

    public override void OnDraw()
    {
        Size = baseSize;
        SizeCondition = ImGuiCond.FirstUseEver;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = minSize,
            MaximumSize = wideSize
        };

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
        if (!BeginListBox("##<stylingboxrenamepannel>", new Vector2(ContentAvailableX - imageBoxSize.X - FramePaddingX, imageBoxSize.Y)))
            return;
        DrawInputFieldInsides();
        ImGui.EndListBox();
    }

    void DrawInputFieldInsides()
    {
        if (user == null) return;
        if (activePet == null) return;

        if (activePet.petID == -1 || activePet.baseName == string.Empty)
        {
            Label($"Please summon a {activePet.referredToAs} or open the naming list", new Vector2(ContentAvailableX, BarSize));
            if (Button("Naming List", new Vector2(ContentAvailableX, BarSize))) PluginLink.WindowHandler.OpenWindow<PetListWindow>();
            SetTooltipHovered($"Opens the {activePet.referredToAs} List");
            return;
        }
        DrawPetNameField();
    }

    void DrawImageBox()
    {
        if (!BeginListBox("##<stylingboxrenamepanne2l>", imageBoxSize))
            return;
        DrawImage(activePet.textureWrap.ImGuiHandle, new Vector2(111, 112));
        ImGui.EndListBox();
    }

    public override void OnDrawSharing()
    {
        PluginLink.WindowHandler.GetWindow<PetListWindow>()?.DrawExportHeader();
    }

    void DrawPetNameField()
    {
        string tempText = $"does not have a name!";
        if (activePet.petName.Length != 0) tempText = $"is named:";
        Label($"Your {activePet.baseName} {tempText}", new Vector2(ContentAvailableX, BarSize), StylingColours.defaultText);
        SetTooltipHovered($"{activePet.baseName}");
        if (activePet.petName.Length != 0)
        {
            Label($"{activePet.petName}", new Vector2(ContentAvailableX, BarSize), StylingColours.defaultText);
            SetTooltipHovered($"{activePet.petName}");
        }
        InputTextMultiLine(string.Empty, ref activePet._temporaryPetName, PluginConstants.ffxivNameSize, new Vector2(ContentAvailableX, BarSize), ImGuiInputTextFlags.CtrlEnterForNewLine);
        SetTooltipHovered("Put in a nickname here.");
        DrawValidName();
    }

    void DrawValidName()
    {
        Button("Save Nickname", new Vector2(ContentAvailableX / 2 - FramePaddingX, 25), "[Required to see a nickname]", Save); ImGui.SameLine(0, 1f);
        Button("Clear Nickname", new Vector2(ContentAvailableX, 25), "[Clears the nickname from your list.]", Delete);
    }

    void Save()
    {
        user.SerializableUser.SaveNickname(activePet.petID, activePet.temporaryPetName);
        OnButton();
    }

    void Delete()
    {
        activePet.temporaryPetName = string.Empty;
        user.SerializableUser.RemoveNickname(activePet.petID);
        OnButton();
    }

    void OnButton()
    {
        activePet.petName = activePet.temporaryPetName;
        PluginLink.Configuration.Save();
        if (activePet.petID > -1) 
        {
            PetBase minion = PluginLink.PettableUserHandler.LocalUser()!.Minion;
            PenumbraIPCProvider.RedrawMinionByIndex(minion.Index);
            SendIPC(minion);
        }
        if (activePet.petID < -1)
        {
            PetBase battlePet = PluginLink.PettableUserHandler.LocalUser()!.BattlePet;
            PenumbraIPCProvider.RedrawBattlePetByIndex(battlePet.Index);
            SendIPC(battlePet);
        }
    }

    void SendIPC(PetBase petBase)
    {
        if (!user.LocalUser) return;
        if (!user.UserChanged) return;
        IpcProvider.NotifySetPetNickname(petBase.Pet, activePet.petName);
    }

    public void OpenForId(int id, bool forceOpen = false)
    {
        if ((user ??= PluginLink.PettableUserHandler.LocalUser()!) == null) return;
        if (forceOpen) ForceOpenForID(id);

        RenamablePet lastPet = activePet;

        if ((activePet = GetPet(FromID(id))) == null) return;

        activePet.petID = id;
        activePet.baseName = RemapUtils.instance.PetIDToName(id).MakeTitleCase();
        activePet.petName = user.GetCustomName(id);
        activePet.temporaryPetName = activePet.petName;

        string iconPath = RemapUtils.instance.GetTextureID(id).GetIconPath();
        activePet.textureWrap = PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!;

        if (!forceOpen)
            activePet = lastPet;
    }

    public void OpenForMinion(int id, bool forceOpen = false)
    {
        if (id == -1) pets[0]?.Clear();
        else OpenForId(id, forceOpen);
    }

    public void OpenForBattlePet(int id, bool forceOpen = false)
    {
        if (id == -1) pets[1]?.Clear();
        else OpenForId(id, forceOpen);
    }

    void ForceOpenForID(int id)
    {
        SetModeForID(id);
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
            if (pet.associatedMode == mode) 
                return pet;
        return null!;
    }

    PetMode FromID(int id)
    {
        if (id == -1) return PetMode.ShareMode;
        if (id < -1) return PetMode.BattlePet;
        if (id > -1) return PetMode.Normal;

        return PetMode.Normal;
    }

    protected override void OnDispose()
    {
        foreach (RenamablePet pet in pets)
            pet?.Dispose();
    }

    public void Reset()
    {
        foreach (var pet in pets)
            pet.Clear();
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

    public void Clear()
    {
        petID = -1;
        baseName = string.Empty;
        temporaryPetName = string.Empty;
        Dispose();
    }

    public void Dispose()
    {
        textureWrap?.Dispose();
        textureWrap = null!;
    }
}
