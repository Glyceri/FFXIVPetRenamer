using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;
using System;
using System.Collections.Generic;
using Action = Lumina.Excel.Sheets.Action;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetSheets
{
    TextCommand?        GetCommand(uint id);
    Action?             GetAction(uint actionId);
    BNpcName?           GetBNpcName(uint bNpcId);
    string?             GetClassName(int id);
    string?             GetWorldName(ushort worldId);
    IPetSheetData?      GetPet(PetSkeleton skeletonId);
    IPetSheetData[]     GetLegacyPets(int legacyModelId);
    List<IPetSheetData> GetMissingPets(List<PetSkeleton> battlePetSkeletons);
    PetSkeleton         ToSoftSkeleton(PetSkeleton skeletonId, PetSkeleton[] softSkeletons);
    IPetSheetData?      GetPetFromName(string name);
    IPetSheetData?      GetPetFromIcon(uint iconId);
    IPetSheetData?      GetPetFromAction(uint actionId, IPettableUser user, bool isSoft = true);
    IPetSheetData?      GetPetFromString(string text, IPettableUser user, bool isSoft = false);
    IPetSheetData       MakeSoft(IPettableUser user, IPetSheetData oldData);
    int?                NameToSoftSkeletonIndex(string name);
    int?                CastToSoftIndex(uint castId);
    bool                IsValidBattlePet(PetSkeleton skeleton);

    [Obsolete] PetSkeleton[] GetObsoleteIDsFromClass(int classJob);
    
    static readonly Dictionary<PetSkeleton, uint> PetIdToAction = new()
    {
        { PluginConstants.EmeraldCarbuncle , 25804 }, // Summon Emerald
        { PluginConstants.RubyCarbuncle    , 25802 }, // Summon Ruby
        { PluginConstants.Carbuncle        , 25798 }, // Summon Carbuncle
        { PluginConstants.TopazCarbuncle   , 25803 }, // Summon Topaz
        { PluginConstants.IfritEgi         , 25805 }, // Summon Ifrit
        { PluginConstants.TitanEgi         , 25806 }, // Summon Titan
        { PluginConstants.GarudaEgi        , 25807 }, // Summon Garuda
        { PluginConstants.Eos              , 17215 }, // Summon Eos
        { PluginConstants.Selene           , 17215 }, // Summon Eos
        { PluginConstants.AutomatonQueen   , 16501 }, // Automaton Queen
        { PluginConstants.Seraph           , 16545 }, // Summon Seraph
        { PluginConstants.Phoenix          , 25831 }, // Summon Phoenix
        { PluginConstants.LivingShadow     , 16472 }, // Living Shadow
        { PluginConstants.IffritII         , 25838 }, // Summon Ifrit II
        { PluginConstants.GarudaII         , 25840 }, // Summon Garuda II
        { PluginConstants.TitanII          , 25839 }, // Summon Titan II
        { PluginConstants.Bahamut          , 7427  }, // Summon Bahamut
        { PluginConstants.RookAutoTurret   , 2864  }, // Rook Autoturret
        { PluginConstants.SolarBahamut     , 36992 }, // Summon Solar Bahamut        
    };
    
    static readonly Dictionary<PetSkeleton, uint> BattlePetToBNpcName = new()
    {
        { PluginConstants.EmeraldCarbuncle , 1401  },
        { PluginConstants.RubyCarbuncle    , 4149  },
        { PluginConstants.Carbuncle        , 10261 },
        { PluginConstants.TopazCarbuncle   , 1400  },
        { PluginConstants.IfritEgi         , 1402  },
        { PluginConstants.TitanEgi         , 1403  },
        { PluginConstants.GarudaEgi        , 1404  },
        { PluginConstants.Eos              , 1398  },
        { PluginConstants.Selene           , 1399  },
        { PluginConstants.AutomatonQueen   , 8230  },
        { PluginConstants.Seraph           , 8227  },
        { PluginConstants.Phoenix          , 8228  },
        { PluginConstants.LivingShadow     , 8229  },
        { PluginConstants.IffritII         , 10262 },
        { PluginConstants.GarudaII         , 10263 },
        { PluginConstants.TitanII          , 10264 },
        { PluginConstants.Bahamut          , 6566  },
        { PluginConstants.RookAutoTurret   , 3666  },
        { PluginConstants.SolarBahamut     , 13159 },
    };

    static readonly Dictionary<uint, PetSkeleton> BattlePetRemap = new()
    {
        { 6,  PluginConstants.Eos              },
        { 7,  PluginConstants.Selene           },

        { 1,  PluginConstants.EmeraldCarbuncle },
        { 38, PluginConstants.RubyCarbuncle    },
        { 2,  PluginConstants.TopazCarbuncle   },
        { 36, PluginConstants.Carbuncle        },

        { 27, PluginConstants.IfritEgi         },
        { 28, PluginConstants.TitanEgi         },
        { 29, PluginConstants.GarudaEgi        },

        { 8,  PluginConstants.RookAutoTurret   },
        { 21, PluginConstants.Seraph           },
        { 18, PluginConstants.AutomatonQueen   },
        { 17, PluginConstants.LivingShadow     },

        { 14, PluginConstants.Phoenix          },
        { 10, PluginConstants.Bahamut          },
        { 32, PluginConstants.GarudaII         },
        { 31, PluginConstants.TitanII          },
        { 30, PluginConstants.IffritII         },
        { 46, PluginConstants.SolarBahamut     },
    };
}
