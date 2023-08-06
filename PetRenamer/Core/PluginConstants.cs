namespace PetRenamer.Core;

public static class PluginConstants
{
    public const string pluginName = "Minion Nicknames";
    public const string mainCommandAlt = "/petname";
    public const string petConfigCommandAlt = "/petconfig";
    public const string mainCommand = "/minionname";
    public const string petConfigCommand = "/minionconfig";
    public const int ffxivNameSize = 64;
    public static readonly int[] allowedJobs = new int[] 
    { 
        26,     //Arcanist
        27,     //Summoner
        28,     //Scholar
//        30,     //Ninja
        31,     //Machinist 
        32,     //Dark Knight
    };
    public static readonly int[] allowedNegativePetIDS = new int[]
    {
        -2,     //Arcanist/Summoner
        -3,     //Scholar
        -4,     //Machinist
        -5,     //Dark Knight
//        -6,     //Ninja
    };
}