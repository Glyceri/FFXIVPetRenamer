using System;
using System.Numerics;
using Dalamud.Game;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;

namespace PetRenamer.Windows;

public class MainWindow : Window, IDisposable
{
    private PetRenamerPlugin Plugin;

    public static string testText = "Pet Name";

    public MainWindow(PetRenamerPlugin plugin) : base(
        "Pet Name", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.Plugin = plugin;

    }

    public void Dispose()
    {
        
    }

    byte[] tempName = new byte[64];

    public override void Draw()
    {

        ImGui.InputText(this.Plugin.Configuration.CustomPetName, tempName, 64);
        
            
        
       // ImGui.Text($"The random config bool is {this.Plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        ImGui.Text(testText);

        if (ImGui.Button("Save Name"))
        {
            Plugin.petName = tempName;
            string s = string.Empty;
            foreach(byte b in tempName)
            {
                s += (char)b;
            }
            this.Plugin.Configuration.CustomPetName = s;
            Plugin.Configuration.Save();
        }

        ImGui.Spacing();

        

       /* ImGui.Text("Have a goat:");
        ImGui.Indent(55);
        ImGui.Image(this.GoatImage.ImGuiHandle, new Vector2(this.GoatImage.Width, this.GoatImage.Height));
        ImGui.Unindent(55);*/
    }
}
