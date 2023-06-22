using System;
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

        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
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

        //tempText = string.Empty;
        ImGui.InputText(tempText, tempName, 64);

        string internalTempText = utils.FromBytes(tempName);

        // ImGui.Text($"The random config bool is {this.Plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        ImGui.Text(Globals.CurrentID.ToString());

        if (ImGui.Button("Save Name"))
        {
            tempText = internalTempText;
            if (!utils.Contains(Globals.CurrentID))
            {
                Plugin.Configuration.nicknames = new SerializableNickname[1];
                Plugin.Configuration.nicknames[0] = new SerializableNickname(Globals.CurrentID, internalTempText);
            }

            SerializableNickname nick = utils.GetNickname(Globals.CurrentID);
            if(nick != null)
                nick.Name = internalTempText;


            Plugin.Configuration.Save();
        }

        //ImGui.Spacing();



        /* ImGui.Text("Have a goat:");
         ImGui.Indent(55);
         ImGui.Image(this.GoatImage.ImGuiHandle, new Vector2(this.GoatImage.Width, this.GoatImage.Height));
         ImGui.Unindent(55);*/
    }
}
