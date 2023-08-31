using ImGuiNET;
using PetRenamer.Windows.Attributes;
using System.Numerics;

namespace PetRenamer.Windows.PetWindows;

[PersistentPetWindow]
[ModeTogglePetWindow]
internal class PetHelpWindow : PetWindow
{
    internal enum HelpMode
    {
        Naming,
        Modes,
        Sharing
    }

    HelpMode curMode { get; set; } = HelpMode.Naming;

    public PetHelpWindow() : base("Pet Nicknames Help Window", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(803, 907),
            MaximumSize = new Vector2(803, 907)
        };

        IsOpen = false;
    }

    public override void OnDraw()
    {
        ImGui.BeginListBox("##BaseBox", new Vector2(790, 60));
        NewLabel("^", Styling.helpButtonSize);
        SameLinePretendSpace();
        NewLabel("Use these buttons to toggle between different modes", Styling.FillSize);
    }

    public override void OnDrawNormal()
    {
        NewLabel("!", Styling.helpButtonSize);
        SameLinePretendSpace();
        Label("You are currently in Minion mode!", Styling.FillSize);
        ImGui.EndListBox();
    }

    public override void OnDrawBattlePet()
    {
        NewLabel("!", Styling.helpButtonSize);
        SameLinePretendSpace();
        Label("You are currently in Battle Pet Mode!", Styling.FillSize);
        ImGui.EndListBox();
    }

    public override void OnDrawSharing()
    {
        NewLabel("!", Styling.helpButtonSize);
        SameLinePretendSpace();
        Label("You are currently in Sharing mode!", Styling.FillSize);
        ImGui.EndListBox();
    }

    public override void OnLateDraw()
    {
        NewLine();
        NewLine();
        BeginListBox("##HelpBoxBase", new Vector2(790, 735));
        DrawHelpHeader();
        if (curMode == HelpMode.Naming)  DrawNamingHelp();
        if (curMode == HelpMode.Modes)   DrawModesHelp();
        if (curMode == HelpMode.Sharing) DrawSharingHelp();

        ImGui.EndListBox();
    }

    void DrawHelpHeader()
    {
        if (Button("Naming a pet", Styling.ListSmallNameField)) curMode = HelpMode.Naming; ImGui.SameLine(0, 91);

        if (ImGui.IsItemHovered()) SetTooltip("Help with naming a pet.");

        if (Button("Modes", Styling.ListSmallNameField)) curMode = HelpMode.Modes; ImGui.SameLine(0, 91);

        if (ImGui.IsItemHovered()) SetTooltip("Help with switching modes and it's implications.");

        if (Button("Sharing", Styling.ListSmallNameField)) curMode = HelpMode.Sharing;

        if (ImGui.IsItemHovered()) SetTooltip("Help with sharing nicknames.");
    }

    void DrawNamingHelp()
    {
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace(); 
        Label("Help with naming a pet.", Styling.FillSize);
        NewLine();
        BeginListBox("##Naming Help", new Vector2(782, 648));

        NewLabel("1.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("You can open the naming window by right-clicking on your minion or pet and choosing the [Give Nickname] option.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("You may also type: [/minionname] or [/petname].", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("It may ask you to summon a pet if one isn't out. Please do as instructed.", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Depending on if you are naming a minion or battle pet you need to switch modes.", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("See the section [Modes] for help with that.", Styling.FillSizeSmall);
        NewLine();
        NewLabel("2.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("Fill a name into the Text Box and click [Save Nickname].", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Not clicking [Save Nickname] will result in the nickname NOT applying.", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("ALL your nicknames will automatically save.", Styling.FillSizeSmall);
        NewLine();
        NewLabel("3.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("If your pet has a nickname and you don't want it to have one anymore? Please click [Remove Nickname].", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("ALL your nicknames will automatically save.", Styling.FillSizeSmall);
        NewLine();

        NewLabel("4.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("There is another way to name a Minion or Battle Pet.", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Open the List window via: ", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("[/minionnames] [/petnames] [/minionlist] [/petlist]", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Or the button in the top right corner of most windows.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Depending on your current mode you can see an overview of all your Minion nicknames or Battle Pet nicknames.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("In Minion Mode you can add a nickname to ANY Minion, even if you do not have that Minion currently summoned.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("By clicking the: [+] button you can filter by Minion ID or by name.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("By clicking the: [+] button associated with a Minion it will automatically add it to the list.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Adding a Minion to the list will automatically bring up the naming window.", Styling.FillSizeSmall);
        ImGui.EndListBox();
    }

    void DrawModesHelp()
    {
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Help about switching modes and it's implications.", Styling.FillSize);

        NewLine();
        BeginListBox("##Modes Help Box", new Vector2(782, 648));

        NewLabel("1.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("In the top left of some windows small buttons will be located.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Clicking these will switch modes.", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Switching modes is required for naming minions or battle pets.", Styling.FillSizeSmall);
        NewLine();
        NewLabel("2.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("There are 3 modes:", Styling.FillSizeSmall);

        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("[Minion Mode]", Styling.FillSizeSmall);

        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("[Battle Pet Mode]", Styling.FillSizeSmall);

        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("[Sharing Mode]", Styling.FillSizeSmall);

        NewLine();
        NewLabel("3.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("[Minion Mode]", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Enable minion mode to give a nickname to minions.", Styling.FillSizeSmall);

        NewLine();
        NewLabel("4.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("[Battle Pet Mode]", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Enable Battle Pet mode to give a nickname to Battle Pets. (AKA: Carbuncle, Faerie)", Styling.FillSizeSmall); 
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Battle Pet mode functionality (mostly) only works when the player is one of the following classes/jobs: ", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("[Arcanist] [Summoner] [Scholar] [Machinist] [Dark Knight]", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Battle Pet names are tied to the players current job not their current Battle Pet", Styling.FillSizeSmall);

        NewLine();
        NewLabel("5.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("[Sharing Mode]", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Enable Sharing mode to export and/or import a list of your, or other peoples, nicknames.", Styling.FillSizeSmall);
        ImGui.EndListBox();
    }

    void DrawSharingHelp()
    {
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Help about sharing your pets with your friends.", Styling.FillSize);

        NewLine();
        BeginListBox("##Sharing Help Box", new Vector2(782, 648));

        NewLabel("1.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("Sharing Mode is by far the most interesting mode. You can access the full Sharing Mode window via: ", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("[/minionnames] [/petnames] [/minionlist] [/petlist]", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Or the button in the top right corner of most windows.", Styling.FillSizeSmall);
        NewLine();

        NewLabel("2.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("Exporting", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("By clicking [Export to Clipboard] you will copy all your petnames to your clipboard.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Send this list to your friends (For example: pasting it into Discord) so they can copy it.", Styling.FillSizeSmall);
        NewLine();

        NewLabel("3.", Styling.helpButtonSize); SameLinePretendSpace();
        NewLabel("Importing", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("By clicking [Import to Clipboard] you will import a list of nicknames from someone else.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("This requires you to have copied the line of text your friend send.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("Upon importing a list will show of all the imported nicknames.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("By clicking [Save Nicknames] in the bottom right corner you will save all the imported nicknames.", Styling.FillSizeSmall);
        NewLabel("!", Styling.helpButtonSize); SameLinePretendSpace();
        Label("This feature allows you to see your friends nicknames!", Styling.FillSizeSmall);
        NewLabel("?", Styling.helpButtonSize); SameLinePretendSpace();
        Label("ALL your nicknames will automatically save.", Styling.FillSizeSmall);

        ImGui.EndListBox();
    }
}
