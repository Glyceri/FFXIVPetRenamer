using System.Numerics;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Updatable.Updatables;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
[ModeTogglePetWindow]
public class MainWindow : InitializablePetWindow
{
    readonly StringUtils stringUtils;
    readonly SheetUtils sheetUtils;
    readonly NicknameUtils nicknameUtils;
    readonly ConfigurationUtils configurationUtils;
    readonly RemapUtils remapUtils;

    int gottenID = -1;
    int gottenBattlePetID = -1;
    string tempName = string.Empty;
    string tempName2 = string.Empty;
    string tempText = string.Empty;
    string tempText2 = string.Empty;

    public MainWindow() : base("Give Nickname", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoFocusOnAppearing)
    {
        Size = new Vector2(310, 195);
        
        stringUtils         = PluginLink.Utils.Get<StringUtils>();
        nicknameUtils       = PluginLink.Utils.Get<NicknameUtils>();
        sheetUtils          = PluginLink.Utils.Get<SheetUtils>();
        configurationUtils  = PluginLink.Utils.Get<ConfigurationUtils>();
        remapUtils          = PluginLink.Utils.Get<RemapUtils>();
    }
    
    public override void OnOpen()
    {
        tempText = string.Empty;
        tempText2 = string.Empty;
        if (nicknameUtils.ContainsLocal(gottenID))
            tempText = stringUtils.GetLocalName(gottenID);
        if (nicknameUtils.ContainsLocal(gottenBattlePetID))
            tempText2 = stringUtils.GetLocalName(gottenBattlePetID);

        tempName2 = tempText2;
        tempName = tempText;
    }

    public override void OnDrawNormal() => DrawNormalMode();
    public override void OnDrawBattlePet() => HandleBattleMode();

    void HandleBattleMode()
    {
        if (gottenBattlePetID >= -1) DrawGetPetOut();
        else HandleBattlePetName();
    }

    void HandleBattlePetName()
    {
        DrawPetNameField(remapUtils.PetIDToName(gottenBattlePetID), ref tempText2, ref tempName2, ref gottenBattlePetID);
    }

    void DrawGetPetOut()
    {
        string gottenString = remapUtils.PetIDToName(gottenBattlePetID);
        if(gottenString ==  string.Empty) ImGui.TextColored(StylingColours.highlightText, $"Please summon a Battle Pet.");
        else ImGui.TextColored(StylingColours.highlightText, $"Please summon your {gottenString}.");
    }

    void DrawNormalMode()
    {
        if (gottenID <= -1) DrawNoMinionSpawned();
        else DrawPetNameField(stringUtils.MakeTitleCase(sheetUtils.GetPetName(gottenID)), ref tempText, ref tempName, ref gottenID);
    }

    void DrawNoMinionSpawned()
    {
        ImGui.TextColored(StylingColours.highlightText, "Please summon a minion.");
    }

    void DrawPetNameField(string basePet, ref string theTempText, ref string theTempName, ref int theID)
    {
        ImGui.TextColored(StylingColours.whiteText, $"Your");
        SameLinePretendSpace();
        ImGui.TextColored(StylingColours.highlightText, $"{basePet}");
        SameLinePretendSpace();
        if (theTempText.Length == 0)
        {
            ImGui.TextColored(StylingColours.whiteText, $"does not have a name!");
        }
        else
        {
            ImGui.TextColored(StylingColours.whiteText, $"is named:");
            if(theTempText.Length < 10)
                SameLinePretendSpace();
            ImGui.TextColored(StylingColours.highlightText, $"{theTempText}");
        }
        InputText(string.Empty, ref theTempName, PluginConstants.ffxivNameSize);

        theTempName = theTempName.Trim();
        DrawValidName(theTempName, ref theID);
    }

    void DrawValidName(string internalTempText, ref int theID)
    {
        if (Button("Save Nickname"))
        {
            
            SerializableNickname nName = nicknameUtils.GetLocalNickname(theID);
            if(nName.Name != internalTempText)
                IpcProvider.ChangedPetNickname(new NicknameData(theID, internalTempText));
            configurationUtils.SetLocalNickname(theID, internalTempText);
            OnOpen();
        }
        ImGui.SameLine(0, 1f);
        if (Button("Remove Nickname"))
        {
            configurationUtils.RemoveLocalNickname(theID);
            OnOpen();
        }
        ImGui.TextColored(StylingColours.whiteText, "Resummon your minion or simply look away \nfor a moment to apply the nickname.");
    }

    unsafe void OnChange(PlayerData? playerData, SerializableNickname nickname)
    {
        gottenID = -1;
        gottenBattlePetID = -1;
        OnOpen();
        if (playerData == null) return;
        Dalamud.Logging.PluginLog.Log("early: " + playerData!.Value.battlePetID.ToString());
        gottenBattlePetID = playerData!.Value.battlePetID;
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

    public void OpenForBattleID(int id)
    {
        gottenBattlePetID = id;
        IsOpen = true;
        OnOpen();
    }

    public override void OnInitialized()
    {
        NameChangeUpdatable dataGatherer = (NameChangeUpdatable)PluginLink.UpdatableHandler.GetElement(typeof(NameChangeUpdatable));
        dataGatherer.RegisterMethod(OnChange!);
    }
}