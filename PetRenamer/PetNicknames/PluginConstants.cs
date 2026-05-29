using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames;

public static class PluginConstants
{
    public const string pluginName         = "Pet Nicknames";

    public const int    ffxivNameSize      = 32;
    public const char   forbiddenCharacter = '^';
    
    public const ulong  InvalidId          = 0xE0000000;

    public static readonly PetSkeleton Eos                    = new PetSkeleton(407,  SkeletonType.BattlePet);
    public static readonly PetSkeleton Selene                 = new PetSkeleton(408,  SkeletonType.BattlePet);
    public static readonly PetSkeleton EmeraldCarbuncle       = new PetSkeleton(409,  SkeletonType.BattlePet);
    public static readonly PetSkeleton RubyCarbuncle          = new PetSkeleton(410,  SkeletonType.BattlePet);
    public static readonly PetSkeleton Carbuncle              = new PetSkeleton(411,  SkeletonType.BattlePet);
    public static readonly PetSkeleton TopazCarbuncle         = new PetSkeleton(412,  SkeletonType.BattlePet);
    public static readonly PetSkeleton IfritEgi               = new PetSkeleton(415,  SkeletonType.BattlePet);
    public static readonly PetSkeleton TitanEgi               = new PetSkeleton(416,  SkeletonType.BattlePet);
    public static readonly PetSkeleton GarudaEgi              = new PetSkeleton(417,  SkeletonType.BattlePet);
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
    
    public static readonly Dictionary<PetSkeleton, uint> PetIdToAction = new()
    {
        { EmeraldCarbuncle , 25804 }, // Summon Emerald
        { RubyCarbuncle    , 25802 }, // Summon Ruby
        { Carbuncle        , 25798 }, // Summon Carbuncle
        { TopazCarbuncle   , 25803 }, // Summon Topaz
        { IfritEgi         , 25805 }, // Summon Ifrit
        { TitanEgi         , 25806 }, // Summon Titan
        { GarudaEgi        , 25807 }, // Summon Garuda
        { Eos              , 17215 }, // Summon Eos
        { Selene           , 17215 }, // Summon Eos
        { AutomatonQueen   , 16501 }, // Automaton Queen
        { Seraph           , 16545 }, // Summon Seraph
        { Phoenix          , 25831 }, // Summon Phoenix
        { LivingShadow     , 16472 }, // Living Shadow
        { IffritII         , 25838 }, // Summon Ifrit II
        { GarudaII         , 25840 }, // Summon Garuda II
        { TitanII          , 25839 }, // Summon Titan II
        { Bahamut          , 7427  }, // Summon Bahamut
        { RookAutoTurret   , 2864  }, // Rook Autoturret
        { SolarBahamut     , 36992 }, // Summon Solar Bahamut        
    };
    
    public static readonly Dictionary<PetSkeleton, uint> BattlePetToBNpcName = new()
    {
        { EmeraldCarbuncle , 1401  },
        { RubyCarbuncle    , 4149  },
        { Carbuncle        , 10261 },
        { TopazCarbuncle   , 1400  },
        { IfritEgi         , 1402  },
        { TitanEgi         , 1403  },
        { GarudaEgi        , 1404  },
        { Eos              , 1398  },
        { Selene           , 1399  },
        { AutomatonQueen   , 8230  },
        { Seraph           , 8227  },
        { Phoenix          , 8228  },
        { LivingShadow     , 8229  },
        { IffritII         , 10262 },
        { GarudaII         , 10263 },
        { TitanII          , 10264 },
        { Bahamut          , 6566  },
        { RookAutoTurret   , 3666  },
        { SolarBahamut     , 13159 },
    };

    public static readonly Dictionary<uint, PetSkeleton> BattlePetRemap = new()
    {
        { 6,  Eos              },
        { 7,  Selene           },

        { 1,  EmeraldCarbuncle },
        { 38, RubyCarbuncle    },
        { 2,  TopazCarbuncle   },
        { 36, Carbuncle        },

        { 27, IfritEgi         },
        { 28, TitanEgi         },
        { 29, GarudaEgi        },

        { 8,  RookAutoTurret   },
        { 21, Seraph           },
        { 18, AutomatonQueen   },
        { 17, LivingShadow     },

        { 14, Phoenix          },
        { 10, Bahamut          },
        { 32, GarudaII         },
        { 31, TitanII          },
        { 30, IffritII         },
        { 46, SolarBahamut     },
    };
    
    [Obsolete("Classes have been obsolete since 1.4")]
    public static readonly Dictionary<int, PetSkeleton[]> BattlePetToClass = new Dictionary<int, PetSkeleton[]>()
    {
        {
            LegacySummonerClassID, 
            [
                Carbuncle,
                RubyCarbuncle,
                TopazCarbuncle,
                EmeraldCarbuncle,
                IfritEgi,
                TitanEgi,
                GarudaEgi,
                IffritII,
                GarudaII,
                TitanII,
                Phoenix,
                Bahamut,
                SolarBahamut
            ]
        },
        {
            LegacyScholarClassID, 
            [
                Eos,
                Selene,
                Seraph
            ]
        },
        {
            LegacyMachinistClassID,
            [
                RookAutoTurret,
                AutomatonQueen,
            ]
        },
        {
            LegacyDarkKnightClassID,
            [
                LivingShadow
            ]
        }
    };
}
