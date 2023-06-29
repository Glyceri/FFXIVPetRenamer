using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Dalamud.Game;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using PetRenamer.Core;
using PetRenamer.Core.Serialization;

namespace PetRenamer.Windows;

public class MainWindow : Window, IDisposable
{
    private PetRenamerPlugin Plugin;

    Utils utils;

    public MainWindow(PetRenamerPlugin plugin, Utils utils) : base(
        "Pet Name", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(400, 140);
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(400, 140),            
            MaximumSize = new Vector2(400, 220)
        };

        this.Plugin = plugin;
        this.utils = utils;
    }

    public void Dispose()
    {

    }

    byte[] tempName = new byte[64];

    string tempText = string.Empty;

    public override void OnOpen()
    {
        tempText = string.Empty;
        if (utils.Contains(Globals.CurrentID))
            tempText = utils.GetName(Globals.CurrentID);

        tempName = utils.GetBytes(tempText);
    }

    public override void Draw()
    {
        if(Globals.CurrentIDChanged) OnOpen();

        if (Globals.CurrentID == -1) { ImGui.Text("Please spawn a pet!"); return; }

        ImGui.TextColored(new Vector4(1,0,1,1), $"Current Pet Name: {tempText}");
        ImGui.InputText(string.Empty, tempName, 64);

        string internalTempText = utils.FromBytes(tempName);



        if (ImGui.Button("Save Name"))
        {
            tempText = internalTempText;
            if (!utils.Contains(Globals.CurrentID))
            {
                List<SerializableNickname> nicknames = Plugin.Configuration.nicknames!.ToList();
                nicknames.Add(new SerializableNickname(Globals.CurrentID, internalTempText));
                Plugin.Configuration.nicknames = nicknames.ToArray();
            }

            SerializableNickname nick = utils.GetNickname(Globals.CurrentID);
            if(nick != null)
                nick.Name = internalTempText;


            Plugin.Configuration.Save();
        }

        if(ImGui.Button("Remove Nickname"))
        {
            if (utils.Contains(Globals.CurrentID))
            {
                List<SerializableNickname> nicknames = Plugin.Configuration.nicknames!.ToList();
                for (int i = nicknames.Count - 1; i >= 0; i--)
                {
                    if (nicknames[i].ID == Globals.CurrentID)
                        nicknames.RemoveAt(i);
                }
                Plugin.Configuration.nicknames = nicknames.ToArray();
                Plugin.Configuration.Save();
                OnOpen();
            }
        }

        if (ImGui.Button("Credits"))
        {
            Plugin.CreditsWindow.IsOpen = true;
        }

        if(Plugin.Debug)
        ImGui.Text("Current Pet ID: " + Globals.CurrentID.ToString());
    }
}
