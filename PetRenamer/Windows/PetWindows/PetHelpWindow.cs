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
        Sharing,
        ThemeEditor,
        PetList
    }

    HelpMode curMode { get; set; } = HelpMode.Naming;

    readonly string[] petListHelpBox1 = new string[]
    {
        "And overview of all the petlist features.",
        "The petlist shows all your nicknamed minions which you can filter/search through.",
        "The petlist shows all your nicknamed battle pets.",
        "The petlist shows a list of all users you added.",
        "The petlist allows you to easily share your list of pet names.",
    };

    readonly string[] openPetlist = new string[]
    {
        "Open the petlist by typing one of the following commands:",
        "/petlist",
        "/petnames",
        "/minionnames",
        "Or by clicking the Square icon located at the top right of most Pet Nickname windows."
    };

    readonly string[] petListDidYouKnow1 = new string[]
    {
        " ? Did you know ? ",
        "Every person you ever added is visible in the pet list. You can see all their nicknames just like you can yours."
    };

    readonly string[] petListDidYouKnow2 = new string[]
    {
        " ? Did you know ? ",
        "You can reorder the petlist to your liking.",
        "Simply start dragging a pet or player in the list.",
        "This will automatically get saved."
    };

    readonly string[] petListHelpBox2 = new string[]
    {
        "Minion Mode Petlist.",
        "In minion mode you see a list of all your nicknamed pets.",
        "By clicking on a pets profile picture or name you can easily and quickly change it's name.",
        "By clicking the little X button you will permanently remove their nickname.",
        "Don't worry. You will get a little confirmation box. And if you accidentally removed a nickname,",
        "you can just as easily give the pet a nickname again.",
        "",
        "Exclusive to the minion mode petlist is the search bar.",
        "The search bar allows you to search through every single minion in the game.",
        "The search bar is located at the top of the list.",
        "Click it and start typing the name (or ID, in case you know those) of a minion.",
        "The list should now reflect every minion that contains a part of your search querry.",
        "By clicking on their empty name bar or the little + button you can give them a nickname.",
        "Very helpful if you don't want to search through the minion guide or have the minion ready."
    };

    readonly string[] petListHelpBox3 = new string[]
    {
        "Battle Pet Mode Petlist.",
        "This works the same as the [Minion Mode Petlist], so I recommend you read that section first.",
        "The only notable differences are that the X button will now not permanently remove a nickname, but clear it.",
        "And the Battle Pet list has no support for a search function. Sorry for that."
    };

    readonly string[] petListHelpBox4 = new string[]
    {
        "Player List",
        "You can navigate to the player list by clicking your user name or profile picture at the top of each list.",
        "(This function does not work in Sharing Mode)",
        "Upon entering Player List mode you will see a list of every player you added.",
        "By clicking on their name or profile picture you will see a list of their minions or battle pets respectively.",
        "Do you want to return to your own pet list. Simply click their profile picture or name at the top like before and click it again.",
        "Double clicking that button will always return you to your own list. You can also search for yourself in the list again.",
        "Useful to know is that the user you are currently logged in with will always display themselves another time at the top of the list.",
        "",
        "There are a couple restrictions when viewing someone else their list.",
        "You can NOT manually remove, alter, or set nicknames. You can also NOT reorder their list.",
        "This is by design. No tinkering with their list. You get what they give.",
        "",
        "Cliking on the X button next to someones name will permanently delete the user.",
        "It will ask you for confirmation, but beware. If you lost the code they shared with you, deleting a user is non-recoverable."
    };

    readonly string[] themingHelpBox1 = new string[]
    {
        "You can open the theme editor window by typing the following command(s).",
        "[/pettheme] or [/miniontheme],",
        "or by clicking the Theme Editor button in the Pet Nicknames Settings window.",
        "Please enable Use Custom Themes in the Pet Nicknames Settings window."
    };

    readonly string[] themingHelpBox2 = new string[]
    {
        "You may switch between Pet Modes to edit their respective themes.",
        "On the left you can choose a colour for a given element.",
        "On the right you will see how the colour looks when applied.",
        "Clicking on Reset Colour will reset that colour back to the last saved colour.",
        "Clicking on Reset To Base Colour will reset that colour back to the colour used in the build in theme.",
        "Please do NOT forget to save by clicking the S button in the top bar."
    };

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
        if (!BeginListBox("##HelpBoxBase", new Vector2(ContentAvailableX, ContentAvailableY))) return;

        DrawHelpHeader();
        if (curMode == HelpMode.Naming) DrawNamingHelp();
        if (curMode == HelpMode.Modes) DrawModesHelp();
        if (curMode == HelpMode.Sharing) DrawSharingHelp();
        if (curMode == HelpMode.ThemeEditor) DrawThemingHelp();
        if (curMode == HelpMode.PetList) DrawPetListHelp();
        ImGui.EndListBox();
    }

    void DrawHelpHeader()
    {
        if (Button("Naming a Pet", "Help with naming a pet.")) curMode = HelpMode.Naming;
        SameLine();
        if (Button("Modes", "Help with switching modes and it's implications.")) curMode = HelpMode.Modes;
        SameLine();
        if (Button("Pet List", "Help with the Pet List and all it's features.")) curMode = HelpMode.PetList;
        SameLine();
        if (Button("Sharing", "Help with sharing nicknames.")) curMode = HelpMode.Sharing;
        SameLine();
        if (Button("Theme Editor", "Help with Editing Themes.")) curMode = HelpMode.ThemeEditor;
    }

    void DrawPetListHelp()
    {
        DrawSimpleBar("!", "Help with the Pet List and all it's features.");
        NewLine();

        DrawBox(openPetlist);
        DrawBox(petListHelpBox1);
        DrawBox(petListDidYouKnow1);
        DrawBox(petListHelpBox2);
        DrawBox(petListDidYouKnow2);
        DrawBox(petListHelpBox3);
        DrawBox(petListHelpBox4);
    }

    void DrawThemingHelp()
    {
        DrawSimpleBar("!", "Help with Editing a Theme.");
        NewLine();

        DrawBox(themingHelpBox1);
        DrawBox(themingHelpBox2);
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

    void DrawType(string type) => OverrideLabel(type, Styling.SmallButton);
}
