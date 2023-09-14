using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class RemapUtils : UtilsRegistryType, ISingletonBase<RemapUtils>
{
    //ClassJob ID's reminder
    //Adventurer        : 0                 Warrior         : 21
    //Gladiator         : 1                 Dragoon         : 22
    //Pugilist          : 2                 Bard            : 23
    //Marauder          : 3                 White Mage      : 24
    //Lancer            : 4                 Black Mage      : 25
    //Archer            : 5                 Arcanist        : 26
    //Conjurer          : 6                 Summoner        : 27
    //Thaumaturge       : 7                 Scholar         : 28
    //Carpenter         : 8                 Rogue           : 29
    //Blacksmith        : 9                 Ninja           : 30
    //Armorer           : 10                Machinist       : 31
    //Goldsmith         : 11                Dark Knight     : 32
    //Leatherworker     : 12                Astrologian     : 33
    //Weaver            : 13                Samurai         : 34
    //Alchemist         : 14                Red Mage        : 35
    //Culinarian        : 15                Blue Mage       : 36
    //Miner             : 16                Gunbreaker      : 37
    //Botanist          : 17                Dancer          : 38
    //Fisher            : 18                Reaper          : 39
    //Paladin           : 19                Sage            : 40
    //Monk              : 20

    internal override void OnLateRegistered()
    {
        foreach (int skeletonID in battlePetRemap.Keys)
            bakedBattlePetSkeletonToName.Add(skeletonID, SheetUtils.instance.GetBattlePetName(skeletonID));
    }
    
    // [Populated]
    public readonly Dictionary<int, string> bakedBattlePetSkeletonToName = new Dictionary<int, string>();

    public readonly Dictionary<int, int> battlePetRemap = new Dictionary<int, int>() 
    {
        { 407,  6  }, //EOS
        { 408,  7  }, //Selene

        { 409,  1  }, //Emerald Carbuncle
        { 410,  38 }, //Ruby Carbuncle
        { 411,  36 }, //Carbuncle
        { 412,  2  }, //Topaz Carbuncle

        { 415,  27 }, //Ifrit-Egi
        { 416,  28 }, //Titan-Egi
        { 417,  29 }, //Garuda-Egi 

        { 1027, 8  }, //Rook Autoturret MCHN
        { 2619, 21 }, //Seraph
        { 2618, 18 }, //Automaton Queen
        { 2621, 17 }, //Esteem DRK

        { 2620, 14 }, //Demi-Phoenix
        { 1930, 10 }, //Demi-Bahamut
        { 3124, 31 }, //Topaz-Titan
        { 3123, 32 }, //Emerald-Garuda
        { 3122, 30 }, //Ruby-Iffrit
    };

    

    public readonly Dictionary<int, int> skeletonToClass = new Dictionary<int, int>()
    {
        { 407,  -3  }, //EOS
        { 408,  -3  }, //Selene

        { 409,  -2  }, //Emerald Carbuncle
        { 410,  -2 }, //Ruby Carbuncle
        { 411,  -2 }, //Carbuncle
        { 412,  -2  }, //Topaz Carbuncle

        { 415,  -2 }, //Ifrit-Egi
        { 416,  -2 }, //Titan-Egi
        { 417,  -2 }, //Garuda-Egi 

        { 1027, -4  }, //Rook Autoturret MCHN
        { 2619, -3 }, //Seraph
        { 2618, -4 }, //Automaton Queen
        { 2621, -5 }, //Esteem DRK

        { 2620, -2 }, //Demi-Phoenix
        { 1930, -2 }, //Demi-Bahamut
        { 3124, -2 }, //Topaz-Titan
        { 3123, -2 }, //Emerald-Garuda
        { 3122, -2 }, //Ruby-Iffrit
    };

    private readonly Dictionary<int, int> classToPetID = new Dictionary<int, int>()
    {
        { 26,   -2 }, //Arcanist to Carbuncle  
        { 27,   -2 }, //Summoner to Carbuncle  
        { 28,   -3 }, //Scholar to Faerie
        { 31,   -4 }, //Machinist to Automaton Queen
        { 32,   -5 }, //Dark Knight Esteem
    };

    private readonly Dictionary<int, string> petIDToPetName = new Dictionary<int, string>()
    {
        { -2, "Carbuncle" },
        { -3, "Faerie" },
        { -4, "Automaton Queen" },
        { -5, "Esteem" },
    };

    public static RemapUtils instance { get; set; } = null!;

    internal string PetIDToName(int petID)
    {
        if (!petIDToPetName.ContainsKey(petID)) return string.Empty;
        return petIDToPetName[petID];
    }

    internal int GetPetIDFromClass(int jobclass)
    {
        if (!classToPetID.ContainsKey(jobclass)) return -1;
        return classToPetID[jobclass];
    }

    internal int BattlePetSkeletonToNameID(int skeletonID)
    {
        if (!battlePetRemap.ContainsKey(skeletonID)) return -1;
        return battlePetRemap[skeletonID];
    }
}

internal enum ClassType
{
    Invalid,
    Summoning,
    Healing,
    Machinist
}
