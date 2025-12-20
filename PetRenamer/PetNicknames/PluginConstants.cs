using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;

namespace PetRenamer.PetNicknames;

public static class PluginConstants
{
    public const string pluginName         = "Pet Nicknames";

    public const int    ffxivNameSize      = 32;
    public const char   forbiddenCharacter = '^';
    
    public const ulong  InvalidId          = 0xE0000000;

    public static readonly PetSkeleton Eos                    = new PetSkeleton(407, SkeletonType.BattlePet);
    public static readonly PetSkeleton Selene                 = new PetSkeleton(408, SkeletonType.BattlePet);
    public static readonly PetSkeleton EmeraldCarbuncle       = new PetSkeleton(409, SkeletonType.BattlePet);
    public static readonly PetSkeleton RubyCarbuncle          = new PetSkeleton(410, SkeletonType.BattlePet);
    public static readonly PetSkeleton Carbuncle              = new PetSkeleton(411, SkeletonType.BattlePet);
    public static readonly PetSkeleton TopazCarbuncle         = new PetSkeleton(412, SkeletonType.BattlePet);
    public static readonly PetSkeleton IfritEgi               = new PetSkeleton(415, SkeletonType.BattlePet);
    public static readonly PetSkeleton TitanEgi               = new PetSkeleton(416, SkeletonType.BattlePet);
    public static readonly PetSkeleton GarudaEgi              = new PetSkeleton(417, SkeletonType.BattlePet);
    public static readonly PetSkeleton RookAutoTurret         = new PetSkeleton(1027, SkeletonType.BattlePet);
    public static readonly PetSkeleton Bahamut                = new PetSkeleton(1930, SkeletonType.BattlePet);
    public static readonly PetSkeleton AutomatonQueen         = new PetSkeleton(2618, SkeletonType.BattlePet);
    public static readonly PetSkeleton Seraph                 = new PetSkeleton(2619, SkeletonType.BattlePet);
    public static readonly PetSkeleton Phoenix                = new PetSkeleton(2620, SkeletonType.BattlePet);
    public static readonly PetSkeleton LivingShadow           = new PetSkeleton(2621, SkeletonType.BattlePet);
    public static readonly PetSkeleton IffritII               = new PetSkeleton(3122, SkeletonType.BattlePet);
    public static readonly PetSkeleton GarudaII               = new PetSkeleton(3123, SkeletonType.BattlePet);
    public static readonly PetSkeleton TitanII                = new PetSkeleton(3124, SkeletonType.BattlePet);
    public static readonly PetSkeleton SolarBahamut           = new PetSkeleton(4038, SkeletonType.BattlePet);

    public static readonly PetSkeleton BaseScholarSkeleton    = Eos;
    public static readonly PetSkeleton BaseSummonerSkeleton   = Carbuncle;
    public static readonly PetSkeleton BaseIfritEgiSkeleton   = IfritEgi;
    public static readonly PetSkeleton BaseTitanEgiSkeleton   = TitanEgi;
    public static readonly PetSkeleton BaseGarudaEgiSkeleton  = GarudaEgi;

    [Obsolete("I stopped using classes in 1.4")] 
    public const int LegacySummonerClassID = -2;

    [Obsolete("I stopped using classes in 1.4")] 
    public const int LegacyScholarClassID = -3;

    [Obsolete("I stopped using classes in 1.4")] 
    public const int LegacyMachinistClassID = -4;

    [Obsolete("I stopped using classes in 1.4")] 
    public const int LegacyDarkKnightClassID = -5;

    // Sheets wrapper explains why the order is like this... it's crucial it stays like this.
    // Soft Mapping is the most hardcoded thing in this plogon :c
    // 0 --> Karfunkel
    // 1 --> Garuda-Egi
    // 2 --> Titan-Egi
    // 3 --> Ifrit-Egi
    // 4 --> Eos
    public static readonly PetSkeleton[] BaseSkeletons 
        = [BaseSummonerSkeleton, BaseGarudaEgiSkeleton, BaseTitanEgiSkeleton, BaseIfritEgiSkeleton, BaseScholarSkeleton];
}
