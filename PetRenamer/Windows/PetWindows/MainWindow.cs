using System.Numerics;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Updatable.Updatables;
using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
[ModeTogglePetWindow]
public class MainWindow : InitializablePetWindow
{
    readonly StringUtils stringUtils;
    readonly SheetUtils sheetUtils;
    readonly NicknameUtils nicknameUtils;
    readonly ConfigurationUtils configurationUtils;

    int gottenID = -1;
    int gottenBattlePetID = -1;
    int gottenClass = -1;
    string tempName = string.Empty;
    string tempText = string.Empty;

    public MainWindow() : base("Give Nickname", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoFocusOnAppearing)
    {
        Size = new Vector2(310, 195);

        IsOpen = true;

        stringUtils         = PluginLink.Utils.Get<StringUtils>();
        nicknameUtils       = PluginLink.Utils.Get<NicknameUtils>();
        sheetUtils          = PluginLink.Utils.Get<SheetUtils>();
        configurationUtils  = PluginLink.Utils.Get<ConfigurationUtils>();
    }
    
    public override void OnOpen()
    {
        tempText = string.Empty;
        if (nicknameUtils.ContainsLocal(gottenID))
            tempText = stringUtils.GetLocalName(gottenID);

        tempName = tempText;
    }

    public override void OnDrawNormal() => DrawNormalMode();
    public override void OnDrawBattlePet() => DrawBattlePetMode();

    void DrawBattlePetMode()
    {
        if (gottenClass < 26 || gottenClass > 28) DrawWrongClass();
        else HandleBattleMode(); 
    }

    void HandleBattleMode()
    {
        if (gottenBattlePetID >= -1) DrawGetPetOut();
        else HandleBattlePetName();
    }

    void HandleBattlePetName()
    {
        if(gottenBattlePetID == -3) ImGui.TextColored(StylingColours.highlightText, "FAIRY FOUND!");
        if (gottenBattlePetID == -2) ImGui.TextColored(StylingColours.highlightText, "FAIRY CARBUNCLE!");
    }

    void DrawGetPetOut()
    {
        if(gottenClass == 28)  ImGui.TextColored(StylingColours.highlightText, "Please summon your Fairy");
        else ImGui.TextColored(StylingColours.highlightText, "Please summon your Carbuncle");
    }

    void DrawWrongClass()
    {
        ImGui.TextColored(StylingColours.highlightText, "Please switch to: ");
        ImGui.TextColored(StylingColours.highlightText, "- Arcanist");
        ImGui.TextColored(StylingColours.highlightText, "- Summoner");
        ImGui.TextColored(StylingColours.highlightText, "- Scholar");
    }

    void DrawNormalMode()
    {
        if (gottenID <= -1) DrawNoMinionSpawned();
        else DrawPetNameField();
    }

    void DrawNoMinionSpawned()
    {
        ImGui.TextColored(StylingColours.highlightText, "Please spawn a Minion!");
    }

    void DrawPetNameField()
    {
        string basePetName = stringUtils.MakeTitleCase(sheetUtils.GetPetName(gottenID));
        ImGui.TextColored(StylingColours.defaultText, $"Your");
        SameLinePretendSpace();
        ImGui.TextColored(StylingColours.highlightText, $"{basePetName}");
        SameLinePretendSpace();
        if (tempText.Length == 0)
        {
            ImGui.TextColored(StylingColours.defaultText, $"does not have a name!");
        }
        else
        {
            ImGui.TextColored(StylingColours.defaultText, $"is named:");
            if(tempText.Length < 10)
                SameLinePretendSpace();
            ImGui.TextColored(StylingColours.highlightText, $"{tempText}");
        }
        InputText(string.Empty, ref tempName, PluginConstants.ffxivNameSize);

        tempName = tempName.Trim();
        DrawValidName(tempName);
    }

    void DrawValidName(string internalTempText)
    {
        if (Button("Save Nickname"))
        {
            configurationUtils.SetLocalNickname(gottenID, internalTempText);
            OnOpen();
        }
        ImGui.SameLine(0, 1f);
        if (Button("Remove Nickname"))
        {
            configurationUtils.RemoveLocalNickname(gottenID);
            OnOpen();
        }
        ImGui.TextColored(StylingColours.defaultText, "Resummon your minion or simply look away \nfor a moment to apply the nickname.");
    }

    unsafe void OnChange(PlayerData? playerData, SerializableNickname nickname)
    {
        gottenID = -1;
        gottenClass = -1;
        gottenBattlePetID = -1;
        OnOpen();
        if (playerData == null) return;
        gottenBattlePetID = playerData!.Value.battlePetID;
        Dalamud.Logging.PluginLog.Log(gottenBattlePetID.ToString());
        gottenClass = ((Character*)playerData!.Value.playerGameObject)->CharacterData.ClassJob;
        OnOpen();
        if (playerData!.Value.companionData == null) return;
        gottenID = playerData!.Value.companionData!.Value.currentModelID;
        OnOpen();
    }

    public void OpenForId(int ID)
    {
        gottenID = ID;
        IsOpen = true;
        OnOpen();
    }

    public override void OnInitialized()
    {
        NameChangeUpdatable dataGatherer = (NameChangeUpdatable)PluginLink.UpdatableHandler.GetElement(typeof(NameChangeUpdatable));
        dataGatherer.RegisterMethod(OnChange!);
    }
}