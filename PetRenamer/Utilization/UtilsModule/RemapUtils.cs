using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public readonly Dictionary<int, uint> petIDToAction = new Dictionary<int, uint>()
    {
        { 409, 25798 },
        { 410, 25798 },
        { 411, 25798 },
        { 412, 25798 },
        { 413, 25798 },
        { 414, 25798 },
        { 415, 25798 },
        { 416, 25798 },
        { 417, 25798 },
        { 407, 17215 },
        { 408, 17215 },
        { 2618, 16501 },
        { 2621, 16472 }
    };

    // [Populated]
    public readonly Dictionary<int, string> bakedBattlePetSkeletonToName = new Dictionary<int, string>();

    public readonly Dictionary<int, int> battlePetRemap = new Dictionary<int, int>()
    {
        { 407,  6  }, //EOS
        { 408,  7  }, //Selene

        { 2619, 21 }, //Seraph

        { 409,  1  }, //Emerald Carbuncle
        { 410,  38 }, //Ruby Carbuncle
        { 411,  36 }, //Carbuncle
        { 412,  2  }, //Topaz Carbuncle

        { 415,  27 }, //Ifrit-Egi
        { 416,  28 }, //Titan-Egi
        { 417,  29 }, //Garuda-Egi 

        { 1027, 8  }, //Rook Autoturret MCHN

        { 2618, 18 }, //Automaton Queen
        { 2621, 17 }, //Esteem DRK
        { 2620, 14 }, //Demi-Phoenix
        { 1930, 10 }, //Demi-Bahamut
        { 3124, 31 }, //Topaz-Titan
        { 3123, 32 }, //Emerald-Garuda
        { 3122, 30 }, //Ruby-Iffrit
    };

    readonly int[][] groupers = new int[][]
    { 
        // The first in the list is what I like to call the group boss

        // Eos and Selene Share a group
        new int[] { 407, 408 }, 
        // Emerald Carbuncle, Ruby Carbuncle, Carbuncle, Topaz Carbuncle, Ifrit-Egi, Titan-Egi and Garuda-Egi share a group
        new int[] { 411, 409, 410, 412, 415, 416, 417 }
    };

    public int[] SharesGroupWith(int id)
    {
        foreach (int[] group in groupers)
            if (group.Contains(id))
                return group;

        return new int[] { id };
    }

    public static RemapUtils instance { get; set; } = null!;

    internal string PetIDToName(int petID, bool grouped = true)
    {
        if (grouped) petID = SharesGroupWith(petID).First();
        if (!bakedBattlePetSkeletonToName.ContainsKey(petID)) return string.Empty;
        return bakedBattlePetSkeletonToName[petID];
    }

    internal uint GetTextureID(int companionID)
    {
        if (companionID > 8000)
        {
            foreach (Companion companion in SheetUtils.instance.petSheet)
            {
                if (companion == null) continue;
                if (companion.Model!.Value!.Model! == companionID)
                    return companion.Icon;
            }
        }
        else if (companionID < 8000)
        {
            if (!petIDToAction.ContainsKey(companionID)) return 786;
            return SheetUtils.instance.actions.GetRow(petIDToAction[companionID])?.Icon ?? 786;
        }

        return 786;
    }
}

internal enum ClassType
{
    Invalid,
    Summoning,
    Healing,
    Machinist
}
