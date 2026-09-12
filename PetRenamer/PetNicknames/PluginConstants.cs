using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.LanguageBased.Values;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System.Numerics;

namespace PetRenamer.PetNicknames;

internal static class PluginConstants
{
    public const string pluginName         = "Pet Nicknames";

    public const int    ffxivNameSize      = 32;
    public const char   forbiddenCharacter = '^';
    
    public const ulong  InvalidId          = 0xE0000000;

    public const string KOFI_URL           = "https://ko-fi.com/glyceri";

    public const uint LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT = 0xAA446688;
    
    public static readonly uint[] HornActions =
    [
        44881,
        44892,
        44894
    ];
    
    public  static readonly PetRegistration Eos                 = new PetRegistration(pet: 6,   modelChara: 407,   bnpcName: 1398,   action: 17215,  SkeletonType.BattlePet, LegacySkeletonType.Scholar);
    private static readonly PetRegistration Selene              = new PetRegistration(pet: 7,   modelChara: 408,   bnpcName: 1399,   action: 17215,  SkeletonType.BattlePet, LegacySkeletonType.Scholar);
    private static readonly PetRegistration Seraph              = new PetRegistration(pet: 21,  modelChara: 2619,  bnpcName: 8227,   action: 16545,  SkeletonType.BattlePet, LegacySkeletonType.Scholar);
    
    private static readonly PetRegistration EmeraldCarbuncle    = new PetRegistration(pet: 26,  modelChara: 409,   bnpcName: 1401,   action: 25804,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    private static readonly PetRegistration RubyCarbuncle       = new PetRegistration(pet: 24,  modelChara: 410,   bnpcName: 4149,   action: 25802,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    public  static readonly PetRegistration Carbuncle           = new PetRegistration(pet: 23,  modelChara: 411,   bnpcName: 10261,  action: 25798,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    private static readonly PetRegistration TopazCarbuncle      = new PetRegistration(pet: 25,  modelChara: 412,   bnpcName: 1400,   action: 25803,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    
    public  static readonly PetRegistration IfritEgi            = new PetRegistration(pet: 27,  modelChara: 415,   bnpcName: 1402,   action: 25805,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    public  static readonly PetRegistration TitanEgi            = new PetRegistration(pet: 28,  modelChara: 416,   bnpcName: 1403,   action: 25806,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    public  static readonly PetRegistration GarudaEgi           = new PetRegistration(pet: 29,  modelChara: 417,   bnpcName: 1404,   action: 25807,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    
    private static readonly PetRegistration IffritII            = new PetRegistration(pet: 30,  modelChara: 3122,  bnpcName: 10262,  action: 25838,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    private static readonly PetRegistration GarudaII            = new PetRegistration(pet: 32,  modelChara: 3123,  bnpcName: 10263,  action: 25840,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    private static readonly PetRegistration TitanII             = new PetRegistration(pet: 31,  modelChara: 3124,  bnpcName: 10264,  action: 25839,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    
    private static readonly PetRegistration Phoenix             = new PetRegistration(pet: 14,  modelChara: 2620,  bnpcName: 8228,   action: 25831,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    private static readonly PetRegistration Bahamut             = new PetRegistration(pet: 10,  modelChara: 1930,  bnpcName: 6566,   action: 7427,   SkeletonType.BattlePet, LegacySkeletonType.Summoner);
    private static readonly PetRegistration SolarBahamut        = new PetRegistration(pet: 46,  modelChara: 4038,  bnpcName: 13159,  action: 36992,  SkeletonType.BattlePet, LegacySkeletonType.Summoner);

    private static readonly PetRegistration RookAutoTurret      = new PetRegistration(pet: 8,   modelChara: 1027,  bnpcName: 3666,   action: 2864,   SkeletonType.BattlePet, LegacySkeletonType.Machinist);
    private static readonly PetRegistration AutomatonQueen      = new PetRegistration(pet: 18,  modelChara: 2618,  bnpcName: 8230,   action: 16501,  SkeletonType.BattlePet, LegacySkeletonType.Machinist);
    
    private static readonly PetRegistration LivingShadow        = new PetRegistration(pet: 17,  modelChara: 2621,  bnpcName: 8229,   action: 16472,  SkeletonType.BattlePet, LegacySkeletonType.DarkKnight);
    
    
    
    private static readonly PetRegistration BMCuSith            = new PetRegistration(pet: 1, modelChara: 4867, bnpcName: 14407,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMSquirrel          = new PetRegistration(pet: 2, modelChara: 25, bnpcName: 14408,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMLamb              = new PetRegistration(pet: 3, modelChara: 287, bnpcName: 14409,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMPugil             = new PetRegistration(pet: 4, modelChara: 356, bnpcName: 14410,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMOpoOpo            = new PetRegistration(pet: 5, modelChara: 31, bnpcName: 14411,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMDodo              = new PetRegistration(pet: 6, modelChara: 173, bnpcName: 14412,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMCoblyn            = new PetRegistration(pet: 7, modelChara: 176, bnpcName: 14413,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMDiremite          = new PetRegistration(pet: 8, modelChara: 21, bnpcName: 14414,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMMegalocrab        = new PetRegistration(pet: 9, modelChara: 148, bnpcName: 14415,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMWespe             = new PetRegistration(pet: 10, modelChara: 359, bnpcName: 14416,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMVulture           = new PetRegistration(pet: 11, modelChara: 39, bnpcName: 14417,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMMandragora        = new PetRegistration(pet: 12, modelChara: 297, bnpcName: 14418,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMGeshunpest        = new PetRegistration(pet: 13, modelChara: 2185, bnpcName: 14419,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMPuk               = new PetRegistration(pet: 14, modelChara: 130, bnpcName: 14420,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMCrab              = new PetRegistration(pet: 15, modelChara: 355, bnpcName: 14421,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMMantis            = new PetRegistration(pet: 16, modelChara: 374, bnpcName: 14422,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMSlime             = new PetRegistration(pet: 17, modelChara: 292, bnpcName: 14423,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMDullahan          = new PetRegistration(pet: 18, modelChara: 121, bnpcName: 14424,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMBat               = new PetRegistration(pet: 19, modelChara: 98, bnpcName: 14425,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMFlyingTrap        = new PetRegistration(pet: 20, modelChara: 48, bnpcName: 14426,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMZiz               = new PetRegistration(pet: 21, modelChara: 157, bnpcName: 14427,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMSabotender        = new PetRegistration(pet: 22, modelChara: 142, bnpcName: 14428,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMGolem             = new PetRegistration(pet: 23, modelChara: 81, bnpcName: 14429,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMApkallu           = new PetRegistration(pet: 24, modelChara: 190, bnpcName: 14430,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMAdamantoise       = new PetRegistration(pet: 25, modelChara: 94, bnpcName: 14431,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMBuffalo           = new PetRegistration(pet: 26, modelChara: 138, bnpcName: 14432,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMUragnite          = new PetRegistration(pet: 27, modelChara: 364, bnpcName: 14433,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMWorm              = new PetRegistration(pet: 28, modelChara: 238, bnpcName: 14434,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMSpriggan          = new PetRegistration(pet: 29, modelChara: 109, bnpcName: 14435,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMGoobbue           = new PetRegistration(pet: 30, modelChara: 198, bnpcName: 14436,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMGigantoad         = new PetRegistration(pet: 31, modelChara: 126, bnpcName: 14437,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMColibri           = new PetRegistration(pet: 32, modelChara: 360, bnpcName: 14438,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMCoeurl            = new PetRegistration(pet: 33, modelChara: 65, bnpcName: 14439,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMRaptor            = new PetRegistration(pet: 34, modelChara: 96, bnpcName: 14440,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMDrake             = new PetRegistration(pet: 35, modelChara: 179, bnpcName: 14441,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMTreant            = new PetRegistration(pet: 36, modelChara: 104, bnpcName: 14442,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMAntling           = new PetRegistration(pet: 37, modelChara: 195, bnpcName: 14443,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMChimera           = new PetRegistration(pet: 38, modelChara: 197, bnpcName: 14444,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMMorbol            = new PetRegistration(pet: 39, modelChara: 145, bnpcName: 14445,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMGhost             = new PetRegistration(pet: 40, modelChara: 264, bnpcName: 14446,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMSalamander        = new PetRegistration(pet: 41, modelChara: 151, bnpcName: 14447,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMCobra             = new PetRegistration(pet: 42, modelChara: 235, bnpcName: 14448,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMHydra             = new PetRegistration(pet: 43, modelChara: 247, bnpcName: 14449,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMDamselfly         = new PetRegistration(pet: 44, modelChara: 654, bnpcName: 14450,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMRottingGoobbue    = new PetRegistration(pet: 45, modelChara: 640, bnpcName: 14451,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMZu                = new PetRegistration(pet: 46, modelChara: 4925, bnpcName: 14452,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMIceGolem          = new PetRegistration(pet: 47, modelChara: 825, bnpcName: 14453,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMKarlabos          = new PetRegistration(pet: 48, modelChara: 824, bnpcName: 14454,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMRafflesia         = new PetRegistration(pet: 49, modelChara: 655, bnpcName: 14455,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    private static readonly PetRegistration BMBehemoth          = new PetRegistration(pet: 50, modelChara: 165, bnpcName: 14456,  action: LOCAL_BATTLE_HORN_ABILITY_REPLACEMENT, SkeletonType.BeastMaster);
    
    public static readonly PetRegistration[] BeastMasterPetRegistrations =
    [
        BMCuSith,
        BMSquirrel,
        BMLamb,
        BMPugil,
        BMOpoOpo,
        BMDodo,
        BMCoblyn,
        BMDiremite,
        BMMegalocrab,
        BMWespe,
        BMVulture,
        BMMandragora,
        BMGeshunpest,
        BMPuk,
        BMCrab,
        BMMantis,
        BMSlime,
        BMDullahan,
        BMBat,
        BMFlyingTrap,
        BMZiz,
        BMSabotender,
        BMGolem,
        BMApkallu,
        BMAdamantoise,
        BMBuffalo,
        BMUragnite,
        BMWorm,
        BMSpriggan,
        BMGoobbue,
        BMGigantoad,
        BMColibri,
        BMCoeurl,
        BMRaptor,
        BMDrake,
        BMTreant,
        BMAntling,
        BMChimera,
        BMMorbol,
        BMGhost,
        BMSalamander,
        BMCobra,
        BMHydra,
        BMDamselfly,
        BMRottingGoobbue,
        BMZu,
        BMIceGolem,
        BMKarlabos,
        BMRafflesia,
        BMBehemoth,
    ];
    
    public static readonly PetRegistration[] BattlePetRegistrations = 
    [
        Eos,
        Selene,
        Seraph,
        
        EmeraldCarbuncle,
        RubyCarbuncle,
        Carbuncle,
        TopazCarbuncle,
        
        IfritEgi,
        TitanEgi,
        GarudaEgi,
        
        IffritII,
        GarudaII,
        TitanII,
        
        Phoenix,
        Bahamut,
        SolarBahamut,
        
        RookAutoTurret,
        AutomatonQueen,
        
        LivingShadow,
    ];
    
    public static readonly PetRegistration[] AllRegistrations =
    [
        .. BattlePetRegistrations,
        .. BeastMasterPetRegistrations,
    ];
    
    public static readonly ModeToggleRegistration MinionModeToggle 
        = new ModeToggleRegistration(SkeletonType.Minion, new Vector3(0.5f, 0.5f, 1.0f), new Vector3(0.36f, 0.36f, 1.0f), new Vector3(0.3f, 0.3f, 0.45f));
    
    public static readonly ModeToggleRegistration BattleModeToggle
        = new ModeToggleRegistration(SkeletonType.BattlePet, new Vector3(0.5f, 1.0f, 0.5f), new Vector3(0.36f, 1.0f,  0.36f), new Vector3(0.3f, 0.45f, 0.3f));
    
    public static readonly ModeToggleRegistration BeastMasterModeToggle
        = new ModeToggleRegistration(SkeletonType.BeastMaster, new Vector3(1.0f, 0.5f, 0.5f), new Vector3(1.0f,  0.36f, 0.36f), new Vector3(0.45f, 0.3f, 0.3f));

    public const string EnglishSummonValue     = "Summon ";      // The space is important.
    public const string GermanSummonValue      = "-Beschwörung"; // The - is important.
    public const string FrenchSummonValue      = "Invocation ";  // The space is important.
    public const string JapaneseSummonValue    = "サモン・";      // The ・ is important.
    public const string ChineseSummonValue     =  "召唤";
    public const string ChineseTradSummonValue =  "召唤";
    public const string KoreanSummonValue      =  " 소환";
    public const string ThaiSummonValue        =  "召唤"; // TODO: FIGURE OUT THE ACTUAL THAI TEXT
    
    public static readonly SummonLanguageValue SummonLanguageValue = new SummonLanguageValue()
    {
        EnglishValue            = EnglishSummonValue,
        GermanValue             = GermanSummonValue,
        FrenchValue             = FrenchSummonValue,
        JapaneseValue           = JapaneseSummonValue,
        ChineseSimplifiedValue  = ChineseSummonValue,
        ChineseTraditionalValue = ChineseTradSummonValue,
        KoreanValue             = KoreanSummonValue,
        TaiwaneseValue          = ThaiSummonValue,
    };

    // Sheets wrapper explains why the order is like this... it's crucial it stays like this.
    // Soft Mapping is the most hardcoded thing in this plogon :c
    // 0 --> Karfunkel
    // 1 --> Garuda-Egi
    // 2 --> Titan-Egi
    // 3 --> Ifrit-Egi
    // 4 --> Eos
    public static readonly PetSkeleton[] BaseSkeletons 
        = [Carbuncle.PetSkeleton, GarudaEgi.PetSkeleton, TitanEgi.PetSkeleton, IfritEgi.PetSkeleton, Eos.PetSkeleton];
}
