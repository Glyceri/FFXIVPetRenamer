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
    NicknameUtils nicknameUtils;

    public MainWindow() : base(
        "Pet Name", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        Size = new Vector2(400, 140);
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(400, 140),
            MaximumSize = new Vector2(400, 11220)
        };

        stringUtils = PluginLink.Utils.Get<StringUtils>();
        nicknameUtils = PluginLink.Utils.Get<NicknameUtils>();
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

        ImGui.TextColored(new Vector4(1, 0, 1, 1), $"Your {PluginLink.Utils.Get<SheetUtils>().GetCurrentPetName()} is named: {tempText}");
        ImGui.InputText(string.Empty, tempName, PluginConstants.ffxivNameSize);

        string internalTempText = stringUtils.FromBytes(tempName);



        if (ImGui.Button("Save Nickname"))
        {
            tempText = internalTempText;
            if (!nicknameUtils.Contains(Globals.CurrentID))
            {
                List<SerializableNickname> nicknames = PluginLink.Configuration.nicknames!.ToList();
                nicknames.Add(new SerializableNickname(Globals.CurrentID, internalTempText));
                PluginLink.Configuration.nicknames = nicknames.ToArray();
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
                List<SerializableNickname> nicknames = PluginLink.Configuration.nicknames!.ToList();
                for (int i = nicknames.Count - 1; i >= 0; i--)
                {
                    if (nicknames[i].ID == Globals.CurrentID)
                        nicknames.RemoveAt(i);
                }
                PluginLink.Configuration.nicknames = nicknames.ToArray();
                PluginLink.Configuration.Save();
                OnOpen();
            }
        }

        if (PluginLink.PetRenamerPlugin.Debug)
            ImGui.Text("Current Pet ID: " + Globals.CurrentID.ToString());
    }
}
