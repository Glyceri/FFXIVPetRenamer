using System.Numerics;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using PetRenamer.Core.Updatable.Updatables;
using System.Collections.Generic;
using PetRenamer.Utilization.Utils;
using System.Linq;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
public class MainWindow : InitializablePetWindow
{
    readonly StringUtils stringUtils;
    readonly SheetUtils sheetUtils;
    readonly NicknameUtils nicknameUtils;
    readonly ConfigurationUtils configurationUtils;
    readonly MathUtils mathUtils;

    int gottenID = -1;
    string tempName = string.Empty;
    string lastValidName = string.Empty;
    string tempText = string.Empty;

    public MainWindow() : base("Minion Nickname", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoFocusOnAppearing)
    {
        Size = new Vector2(300, 145);
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 145),
            MaximumSize = new Vector2(300, 145),
        };

        stringUtils         = PluginLink.Utils.Get<StringUtils>();
        nicknameUtils       = PluginLink.Utils.Get<NicknameUtils>();
        sheetUtils          = PluginLink.Utils.Get<SheetUtils>();
        configurationUtils  = PluginLink.Utils.Get<ConfigurationUtils>();
        mathUtils           = PluginLink.Utils.Get<MathUtils>();
    }
    
    public override void OnOpen()
    {
        tempText = string.Empty;
        if (nicknameUtils.Contains(gottenID))
            tempText = stringUtils.GetName(gottenID);

        tempName = tempText;
    }

    public override void Draw()
    {
        if (gottenID <= -1) DrawNoMinionSpawned();
        else DrawPetNameField();
    }

    void DrawNoMinionSpawned()
    {
        ImGui.Text("Please spawn a Minion!");
    }

    void DrawPetNameField()
    {
        string basePetName = stringUtils.MakeTitleCase(sheetUtils.GetPetName(gottenID));

        if (tempText.Length == 0) ImGui.TextColored(new Vector4(1, 0, 1, 1), $"Your {basePetName} does not have a name!");
        else ImGui.TextColored(new Vector4(1, 0, 1, 1), $"Your {basePetName} is named: {tempText}");
        ImGui.InputText(string.Empty, ref tempName, PluginConstants.ffxivNameSize);

        tempName = tempName.Trim();

        if (!stringUtils.StringIsInvalidForName(tempName))
        {
            lastValidName = tempName;
            DrawValidName(tempName);
        }
        else
        {
            DrawInvalidName();
            tempName = lastValidName;
            tempName = tempName.Replace("�", "");
        }
    }

    //What a dirty way of doing this :D I hate it :D
    void DrawInvalidName()
    {
        List<BooledString> booledStrings = new List<BooledString>();
        for(int i = 0; i < tempName.Length; i++)
        {
            bool charIsValid = stringUtils.CharIsValidForName(tempName[i]);
            if (booledStrings.Count == 0) booledStrings.Add(new BooledString(tempName[i].ToString(), charIsValid));
            else
            {
                if (booledStrings.Last().boolean != charIsValid) booledStrings.Add(new BooledString(tempName[i].ToString(), charIsValid));
                else booledStrings.Last().str += tempName[i];
            }
        }

        foreach(BooledString booledString in booledStrings)
        {
            ImGui.TextColored(booledString.boolean ? new Vector4(1, 1, 1, 1) : new Vector4(1, 0, 0, 1), booledString.str);
            ImGui.SameLine(0, 0.00001f);
        }
        ImGui.NewLine();
        ImGui.TextColored(new Vector4(1, 0, 0, 1), "Your name cannot contain invalid characters!");
    }

    void DrawValidName(string internalTempText)
    {
        if (ImGui.Button("Save Nickname"))
        {
            configurationUtils.SetNickname(gottenID, internalTempText);
            OnOpen();
        }
        ImGui.SameLine(0, 1f);
        if (ImGui.Button("Remove Nickname"))
        {
            configurationUtils.RemoveNickname(gottenID);
            OnOpen();
        }
        ImGui.TextColored(new Vector4(0.8f, 0.8f, 0.8f, 1.0f),"Resummon your minion or simply look away from it\nfor a moment to apply the nickname.");
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
