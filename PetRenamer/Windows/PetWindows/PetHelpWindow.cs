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

    readonly string[] namingBox1Help = new string[]
    {
        "You can open the naming window by right-clicking on your minion or pet and choosing the [Give Nickname] option.",
        "You may also type: [/minionname] or [/petname].",
        "It may ask you to summon a pet if one isn't out. Please do as instructed.",
        "Depending on if you are naming a minion or battle pet you need to switch modes.",
        "See the section [Modes] for help with that."
    };
    readonly string[] namingBox2Help = new string[]
    {
        "Fill a name into the Text Box and click [Save Nickname].",
        "Not clicking [Save Nickname] will result in the nickname NOT applying.",
        "ALL your nicknames will automatically save.",
    };
    readonly string[] namingBox3Help = new string[]
    {
        "If your pet has a nickname and you don't want it to have one anymore? Please click [Remove Nickname].",
         "ALL your nicknames will automatically save."
    };
    readonly string[] namingBox4Help = new string[]
    {
        "There is another way to name a Minion or Battle Pet.",
        "Open the List window via: ",
        "[/minionnames] [/petnames] [/minionlist] [/petlist]",
        "Or the button in the top right corner of most windows.",
        "Depending on your current mode you can see an overview of all your Minion nicknames or Battle Pet nicknames.",
        "In Minion Mode you can add a nickname to ANY Minion, even if you do not have that Minion currently summoned.",
        "By clicking the: [+] button you can filter by Minion ID or by name.",
        "By clicking the: [+] button associated with a Minion it will automatically add it to the list.",
        "Adding a Minion to the list will automatically bring up the naming window."
    };

    readonly string[] modesBox1Help = new string[]
    {
        "In the top left of some windows small buttons will be located.",
        "Clicking these will switch modes.",
        "Switching modes is required for naming minions or battle pets."
    };
    readonly string[] modesBox2Help = new string[]
    {
        "There are 3 modes:",
        "[Minion Mode]",
        "[Battle Pet Mode]",
        "[Sharing Mode]"
    };
    readonly string[] modesBox3Help = new string[]
    {
        "[Minion Mode]",
        "Enable minion mode to give a nickname to minions."
    };
    readonly string[] modesBox4Help = new string[]
    {
        "[Battle Pet Mode]",
        "Enable Battle Pet mode to give a nickname to Battle Pets. (AKA: Carbuncle, Faerie)",
        "Battle Pet mode functionality (mostly) only works when the player is one of the following classes/jobs: ",
        "[Arcanist] [Summoner] [Scholar] [Machinist] [Dark Knight]",
        "Battle Pet names are tied to the players current job not their current Battle Pet",
    };
    readonly string[] modesBox5Help = new string[]
    {
        "[Sharing Mode]",
        "Enable Sharing mode to export and/or import a list of your, or other peoples, nicknames."
    };

    readonly string[] sharingBox1Help = new string[]
    {
        "Sharing Mode is by far the most interesting mode. You can access the full Sharing Mode window via: ",
        "[/minionnames] [/petnames] [/minionlist] [/petlist]",
        "Or the button in the top right corner of most windows."
    };
    readonly string[] sharingBox2Help = new string[]
    {
        "Exporting",
        "By clicking [Export to Clipboard] you will copy all your petnames to your clipboard.",
        "Send this list to your friends (For example: pasting it into Discord) so they can copy it."
    };
    readonly string[] sharingBox3Help = new string[]
    {
        "Importing",
        "By clicking [Import to Clipboard] you will import a list of nicknames from someone else.",
        "This requires you to have copied the line of text your friend send.",
        "Upon importing a list will show of all the imported nicknames.",
        "By clicking [Save Nicknames] in the bottom right corner you will save all the imported nicknames.",
        "This feature allows you to see your friends nicknames!",
        "ALL your nicknames will automatically save."
    };

    public PetHelpWindow() : base("Pet Nicknames Help Window")
    {
        Size = new Vector2(900, 600);
        SizeCondition = ImGuiCond.Appearing;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(400, 400),
            MaximumSize = new Vector2(9999, 9999)
        };
    }

    int boxCounter = 0;

    public override void OnDraw()
    {
        boxCounter = 0;
        if (!ImGui.BeginListBox("##BaseBox", new Vector2(ContentAvailableX, 60)))
            return;
        DrawStylishBar("^", "Use these buttons to toggle between different modes");
        string curMode = petMode == PetMode.ShareMode ? "Sharing" : petMode == PetMode.BattlePet ? "Battle Pet" : "Minion";
        DrawSimpleBar("!", $"You are currently in {curMode} mode!");
        ImGui.EndListBox();
    }

    public override void OnLateDraw()
    {
        boxCounter = 0;
        NewLine();
        NewLine();
        if (!BeginListBox("##HelpBoxBase", new Vector2(ContentAvailableX, 735))) return;

        DrawHelpHeader();
        if (curMode == HelpMode.Naming) DrawNamingHelp();
        if (curMode == HelpMode.Modes) DrawModesHelp();
        if (curMode == HelpMode.Sharing) DrawSharingHelp();
        ImGui.EndListBox();
    }

    void DrawHelpHeader()
    {
        if (Button("Naming a pet", Styling.ListSmallNameField, "Help with naming a pet.")) curMode = HelpMode.Naming;
        SameLine();
        if (Button("Modes", Styling.ListSmallNameField, "Help with switching modes and it's implications.")) curMode = HelpMode.Modes;
        SameLine();
        if (Button("Sharing", Styling.ListSmallNameField, "Help with sharing nicknames.")) curMode = HelpMode.Sharing;
    }

    void DrawNamingHelp()
    {
        DrawSimpleBar("!", "Help with naming a pet.");
        NewLine();

        DrawBox(namingBox1Help);
        DrawBox(namingBox2Help);
        DrawBox(namingBox3Help);
        DrawBox(namingBox4Help);
    }

    void DrawModesHelp()
    {
        DrawSimpleBar("!", "Help about switching modes and it's implications.");
        NewLine();
        DrawBox(modesBox1Help);
        DrawBox(modesBox2Help);
        DrawBox(modesBox3Help);
        DrawBox(modesBox4Help);
        DrawBox(modesBox5Help);
    }

    void DrawSharingHelp()
    {
        DrawSimpleBar("!", "Help about sharing your pets with your friends.");
        NewLine();
        DrawBox(sharingBox1Help);
        DrawBox(sharingBox2Help);
        DrawBox(sharingBox3Help);
    }

    void DrawBox(string[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            string line = array[i];
            if (i == 0) DrawStylishBar($"{++boxCounter}.", line);
            else DrawSimpleBar(">", line);
        }
        NewLine();
    }

    void DrawStylishBar(string type, string line)
    {
        DrawBarBase(type);
        OverrideLabel(line, new Vector2(ContentAvailableX, BarSize));
    }

    void DrawSimpleBar(string type, string line)
    {
        DrawBarBase(type);
        Label(line, new Vector2(ContentAvailableX, BarSize));
    }

    void DrawBarBase(string type)
    {
        DrawType(type);
        SameLinePretendSpace();
    }

    void DrawType(string type) => OverrideLabel(type, Styling.helpButtonSize);
}
