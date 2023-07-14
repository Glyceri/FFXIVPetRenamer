using System.Linq;
using System.Numerics;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;
using PetRenamer.Core.Updatable.Updatables;
using System.IO;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
public class MainWindow : InitializablePetWindow
{
    StringUtils stringUtils;
    SheetUtils sheetUtils;
    NicknameUtils nicknameUtils;

    int gottenID = -1;

    public MainWindow() : base("Minion Nickname", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(300, 180);
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 180),
            MaximumSize = new Vector2(300, 200)
        };

        stringUtils = PluginLink.Utils.Get<StringUtils>();
        nicknameUtils = PluginLink.Utils.Get<NicknameUtils>();
        sheetUtils = PluginLink.Utils.Get<SheetUtils>();
    }

    byte[] tempName = new byte[PluginConstants.ffxivNameSize];

    string tempText = string.Empty;

    public override void OnOpen()
    {
        tempText = string.Empty;
        if (nicknameUtils.Contains(gottenID))
            tempText = stringUtils.GetName(gottenID);

        tempName = stringUtils.GetBytes(tempText);
    }

    public override void Draw()
    {
        if (gottenID == -1) { ImGui.Text("Please spawn a Minion!"); return; }

        ImGui.TextColored(new Vector4(1, 0, 1, 1), $"Your {stringUtils.MakeTitleCase(sheetUtils.GetCurrentPetName())} is named: {tempText}");
        ImGui.InputText(string.Empty, tempName, PluginConstants.ffxivNameSize);

        string internalTempText = string.Concat(stringUtils.FromBytes(tempName).Split(Path.GetInvalidFileNameChars())).Trim();

        if (ImGui.Button("Save Nickname"))
        {
            tempText = internalTempText;
            if (!nicknameUtils.Contains(gottenID))
            {
                List<SerializableNickname> nicknames = PluginLink.Configuration.users!.ToList();
                nicknames.Add(new SerializableNickname(gottenID, internalTempText));
                PluginLink.Configuration.users = nicknames.ToArray();
            }

            SerializableNickname nick = nicknameUtils.GetNickname(gottenID);
            if (nick != null)
                nick.Name = internalTempText;

            PluginLink.Configuration.Save();
        }

        if (ImGui.Button("Remove Nickname"))
        {
            if (nicknameUtils.Contains(gottenID))
            {
                List<SerializableNickname> nicknames = PluginLink.Configuration.users!.ToList();
                for (int i = nicknames.Count - 1; i >= 0; i--)
                {
                    if (nicknames[i].ID == gottenID)
                        nicknames.RemoveAt(i);
                }
                PluginLink.Configuration.users = nicknames.ToArray();
                PluginLink.Configuration.Save();
                OnOpen();
            }
        }

        ImGui.Text("Resummon your minion or simply look away from it for a moment to apply the nickname.");
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

    public override void OnInitialized()
    {
        NameChangeUpdatable dataGatherer = (NameChangeUpdatable)PluginLink.UpdatableHandler.GetElement(typeof(NameChangeUpdatable));
        dataGatherer.RegisterMethod(OnChange!);
    }
}
