using System.Linq;
using System.Numerics;
using ImGuiNET;
using PetRenamer.Core;
using PetRenamer.Core.Serialization;
using PetRenamer.Core.Handlers;
using PetRenamer.Utilization.UtilsModule;
using PetRenamer.Windows.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
public class MainWindow : PetWindow
{
    StringUtils stringUtils;
    SheetUtils sheetUtils;
    NicknameUtils nicknameUtils;

    public MainWindow() : base("Pet Nickname", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(300, 140);
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 140),
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
        if (nicknameUtils.Contains(Globals.CurrentID))
            tempText = stringUtils.GetName(Globals.CurrentID);

        tempName = stringUtils.GetBytes(tempText);
    }

    public override void Draw()
    {
        if (Globals.CurrentIDChanged) OnOpen();

        if (Globals.CurrentID == -1) { ImGui.Text("Please spawn a pet!"); return; }

        ImGui.TextColored(new Vector4(1, 0, 1, 1), $"Your {stringUtils.MakeTitleCase(sheetUtils.GetCurrentPetName())} is named: {tempText}");
        ImGui.InputText(string.Empty, tempName, PluginConstants.ffxivNameSize);

        string internalTempText = stringUtils.FromBytes(tempName);

        if (ImGui.Button("Save Nickname"))
        {
            tempText = internalTempText;
            if (!nicknameUtils.Contains(Globals.CurrentID))
            {
                List<SerializableNickname> nicknames = PluginLink.Configuration.users!.ToList();
                nicknames.Add(new SerializableNickname(Globals.CurrentID, internalTempText));
                PluginLink.Configuration.users = nicknames.ToArray();
            }

            SerializableNickname nick = nicknameUtils.GetNickname(Globals.CurrentID);
            if (nick != null)
                nick.Name = internalTempText;


            PluginLink.Configuration.Save();
        }

        if (ImGui.Button("Remove Nickname"))
        {
            if (nicknameUtils.Contains(Globals.CurrentID))
            {
                List<SerializableNickname> nicknames = PluginLink.Configuration.users!.ToList();
                for (int i = nicknames.Count - 1; i >= 0; i--)
                {
                    if (nicknames[i].ID == Globals.CurrentID)
                        nicknames.RemoveAt(i);
                }
                PluginLink.Configuration.users = nicknames.ToArray();
                PluginLink.Configuration.Save();
                OnOpen();
            }
        }

        if (PluginLink.PetRenamerPlugin.Debug)
            ImGui.Text("Current Pet ID: " + Globals.CurrentID.ToString());
    }
}
