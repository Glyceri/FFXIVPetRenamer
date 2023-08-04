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

    int gottenID = -1;
    string tempName = string.Empty;
    string tempText = string.Empty;

    public MainWindow() : base("Minion Nickname", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoFocusOnAppearing)
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

    void DrawNormalMode()
    {
        if (gottenID <= -1) DrawNoMinionSpawned();
        else DrawPetNameField();
    }

    void DrawNoMinionSpawned()
    {
        ImGui.TextColored(StylingColours.blueText, "Please spawn a Minion!");
    }

    void DrawPetNameField()
    {
        string basePetName = stringUtils.MakeTitleCase(sheetUtils.GetPetName(gottenID));
        ImGui.TextColored(StylingColours.defaultText, $"Your");
        SameLinePretendSpace();
        ImGui.TextColored(StylingColours.blueText, $"{basePetName}");
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
            ImGui.TextColored(StylingColours.blueText, $"{tempText}");
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

    void OnChange(PlayerData? playerData, SerializableNickname nickname)
    {
        gottenID = -1;
        OnOpen();
        if (playerData == null) return;
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