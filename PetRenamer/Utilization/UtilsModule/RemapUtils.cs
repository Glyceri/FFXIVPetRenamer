using Lumina.Excel.GeneratedSheets;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;
using System;
using System.Collections.Generic;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class RemapUtils : UtilsRegistryType, ISingletonBase<RemapUtils>
{
    public static RemapUtils instance { get; set; } = null!;

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
            bakedBattlePetSkeletonToName.Add(skeletonID, SheetUtils.instance.GetBattlePetName(-skeletonID));

        foreach (int actionID in petIDToAction.Values)
            if (!bakedActionIDToName.ContainsKey(actionID))
                bakedActionIDToName.Add(actionID, SheetUtils.instance.GetAction((uint)actionID).Name.ToString());
    }

    public readonly byte[] pettableClasses = new byte[]
    {
        26,
        27,
        28,
        31,
        32
    };

    public readonly List<int> mutatableID = new List<int>()
    {
        -407, //Eos
        -408, //Selene
        -409, //Emerald Carbuncle
        -410, //Ruby Carbuncle
        -411, //Carbuncle
        -412, //Topaz Carbuncle
        -415, //Ifrit-Egi
        -416, //Titan-Egi
        -417  //Garuda-Egi
    };

    public readonly Dictionary<int, uint> petIDToAction = new Dictionary<int, uint>()
    {
        { -409, 25804 }, //Summon Emerald
        { -410, 25802 }, //Summon Ruby
        { -411, 25798 }, //Summon Carbuncle
        { -412, 25803 }, //Summon Topaz
        { -415, 25805 }, //Summon Ifrit
        { -416, 25806 }, //Summon Titan
        { -417, 25807 }, //Summon Garuda
        { -407, 17215 }, //Summon Eos
        { -408, 17215 }, //Summon Eos
        { -2618, 16501 }, //Automaton Queen
        { -2619, 16545 }, //Summon Seraph
        { -2620, 25831 }, //Summon Phoenix
        { -2621, 16472 }, //Living Shadow
        { -3122, 25838 }, //Summon Ifrit II
        { -3123, 25840 }, //Summon Garuda II
        { -3124, 25839 }, //Summon Titan II
        { -1930, 7427 }, //Summon Bahamut
        { -1027, 2864 }, //Rook Autoturret
    };

    public readonly Dictionary<int, uint> basePetIDToAction = new Dictionary<int, uint>()
    {
        { -411, 25798 }, //Summon Carbuncle
        { -408, 17215 }, //Summon Eos
    };

    // [Populated]
    public readonly Dictionary<int, string> bakedActionIDToName = new Dictionary<int, string>();

    // [Populated]
    public readonly Dictionary<int, string> bakedBattlePetSkeletonToName = new Dictionary<int, string>();

    public readonly Dictionary<int, int> battlePetRemap = new Dictionary<int, int>() 
    {
        { -407,  6  }, //EOS
        { -408,  7  }, //Selene

        { -409,  1  }, //Emerald Carbuncle
        { -410,  38 }, //Ruby Carbuncle
        { -411,  36 }, //Carbuncle
        { -412,  2  }, //Topaz Carbuncle

        { -415,  27 }, //Ifrit-Egi
        { -416,  28 }, //Titan-Egi
        { -417,  29 }, //Garuda-Egi 

        { -1027, 8  }, //Rook Autoturret MCHN
        { -2619, 21 }, //Seraph
        { -2618, 18 }, //Automaton Queen
        { -2621, 17 }, //Esteem DRK

        { -2620, 14 }, //Demi-Phoenix
        { -1930, 10 }, //Demi-Bahamut
        { -3124, 31 }, //Topaz-Titan
        { -3123, 32 }, //Emerald-Garuda
        { -3122, 30 }, //Ruby-Iffrit
    };

    internal string PetIDToName(int petID)
    {
       if (petID < -1 && bakedBattlePetSkeletonToName.TryGetValue(petID, out var name)) return name;
       else if (petID > -1) return SheetUtils.instance.GetPetName(petID);
       return string.Empty;
    }

    internal int BattlePetSkeletonToNameID(int skeletonID)
    {
        if (!battlePetRemap.ContainsKey(skeletonID)) return -1;
        return battlePetRemap[skeletonID];
    }

    internal uint GetTextureID(int companionID)
    {
        if (companionID >= 0)
        {
            foreach (Companion companion in SheetUtils.instance.petSheet)
            {
                if (companion == null) continue;
                if (companion.Model!.Value!.RowId! == companionID)
                    return companion.Icon;
            }
        }else if (companionID <= -2)
        {
            if (!petIDToAction.ContainsKey(companionID)) return 786;
            return SheetUtils.instance.actions.GetRow(petIDToAction[companionID])?.Icon ?? 786;
        }

        return 786;
    }

    #region OBSOLETE
    [Obsolete("We dont use classes anymore")]
    public readonly Dictionary<int, int> battlePetToClass = new Dictionary<int, int>()
    {
        { -407,  -3  }, //EOS
        { -408,  -3  }, //Selene

        { -409,  -2  }, //Emerald Carbuncle
        { -410,  -2 }, //Ruby Carbuncle
        { -411,  -2 }, //Carbuncle
        { -412,  -2  }, //Topaz Carbuncle

        { -415,  -2 }, //Ifrit-Egi
        { -416,  -2 }, //Titan-Egi
        { -417,  -2 }, //Garuda-Egi 

        { -1027, -4 }, //Rook Autoturret MCHN
        { -2619, -3 }, //Seraph
        { -2618, -4 }, //Automaton Queen
        { -2621, -5 }, //Esteem DRK

        { -2620, -2 }, //Demi-Phoenix
        { -1930, -2 }, //Demi-Bahamut
        { -3124, -2 }, //Topaz-Titan
        { -3123, -2 }, //Emerald-Garuda
        { -3122, -2 }, //Ruby-Iffrit
    };
    #endregion
}

internal enum ClassType
{
    Invalid,
    Summoning,
    Healing,
    Machinist
}

public static class RemapUtilsHelper
{
    public static string GetIconPath(this uint textureID) => PluginHandlers.TextureProvider.GetIconPath(textureID) ?? string.Empty;
}
