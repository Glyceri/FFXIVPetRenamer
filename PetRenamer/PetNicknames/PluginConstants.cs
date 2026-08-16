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

    
    public static readonly PetRegistration[] PetRegistrations = 
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
