namespace PetRenamer.Core;

public static class PluginConstants
{
    public const string mainCommandAlt = "/petname";
    public const string petConfigCommandAlt = "/petconfig";
    public const string mainCommand = "/minionname";
    public const string petConfigCommand = "/minionconfig";
    public const int ffxivNameSize = 64;
    public const int AtkUnitBaseUpdateIndex = 42;
    public const char forbiddenCharacter = '^';
    public const int baseSummonerSkeleton = 411;
    public const int baseScholarSkeleton = 407;

    public const int arcanistJob = 26;
    public const int summonerJob = 27;
    public const int scholarJob = 28;

    public static readonly int[] allowedJobs = new int[]
    {
        26,     //Arcanist
        27,     //Summoner
        28,     //Scholar
        31,     //Machinist 
        32,     //Dark Knight
    };
    public static readonly int[] allowedNegativePetIDS = new int[]
    {
        -2,     //Arcanist/Summoner
        -3,     //Scholar
        -4,     //Machinist
        -5,     //Dark Knight
    };

    public static readonly string[] removeables = new string[] 
    {
        "the ",
        "den ",
        "des ",
        "dem ",
        "die ",
        "der ",
        "das ",
        "le ",
        "la ",
        string.Empty, //Always put last
    };

    public static class Strings
    {
        public const string exportTooltip = "Exports ALL your nicknames to a list.\nYou can send this list to anyone.\nFor example: Paste this text into Discord and let a friend copy it.\n\n[Hold L-Shift for advanced options.]";
        public const string importTooltip = "After having copied a list of names from a friend.\nClicking this button will result into importing all their nicknames \nallowing you to see them for yourself.";
        public const string deleteUserTooltip = "Will delete this user and all their nicknames from your savefile!";
        public const string keepUserTooltip = "Will keep this user and all their nicknames saved to your savefile!";
        public const string userListPfpWarning = "Help! I cannot see any profile pictures. Please enable: [Allow Automatic Profile Pictures] in the Settings menu.";
    }
}