using System;

namespace PetRenamer.PetNicknames;

public static class PluginConstants
{
    public const string pluginName = "Pet Nicknames";

    public const int ffxivNameSize = 16;
    public const char forbiddenCharacter = '^';

    public const int Eos = -407;
    public const int Selene = -408;
    public const int EmeraldCarbuncle = -409;
    public const int RubyCarbuncle = -410;
    public const int Carbuncle = -411;
    public const int TopazCarbuncle = -412;
    public const int IfritEgi = -415;
    public const int TitanEgi = -416;
    public const int GarudaEgi = -417;
    public const int RookAutoTurret = -1027;
    public const int Bahamut = -1930;
    public const int AutomatonQueen = -2618;
    public const int Seraph = -2619;
    public const int Phoenix = -2620;
    public const int LivingShadow = -2621;
    public const int IffritII = -3122;
    public const int GarudaII = -3123;
    public const int TitanII = -3124;
    public const int SolarBahamut = -4038;

    public const int BaseScholarSkeleton = Eos;
    public const int BaseSummonerSkeleton = Carbuncle;
    public const int BaseIfritEgiSkeleton = IfritEgi;
    public const int BaseTitanEgiSkeleton = TitanEgi;
    public const int BaseGarudaEgiSkeleton = GarudaEgi;

    [Obsolete("I stopped using classes in 1.4")] public const int LegacySummonerClassID = -2;
    [Obsolete("I stopped using classes in 1.4")] public const int LegacyScholarClassID = -3;
    [Obsolete("I stopped using classes in 1.4")] public const int LegacyMachinistClassID = -4;
    [Obsolete("I stopped using classes in 1.4")] public const int LegacyDarkKnightClassID = -5;

    // Sheets wrapper explains why the order is like this... it's crucial it stays like this.
    // Soft Mapping is the most hardcoded thing in this plogon :c
    // 0 --> Karfunkel
    // 1 --> Garuda-Egi
    // 2 --> Titan-Egi
    // 3 --> Ifrit-Egi
    // 4 --> Eos
    public static readonly int[] BaseSkeletons = new int[5] { BaseSummonerSkeleton, BaseGarudaEgiSkeleton, BaseTitanEgiSkeleton, BaseIfritEgiSkeleton, BaseScholarSkeleton };
}
