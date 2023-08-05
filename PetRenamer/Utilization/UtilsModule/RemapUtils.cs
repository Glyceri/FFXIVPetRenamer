using PetRenamer.Utilization.Attributes;
using System.Collections.Generic;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class RemapUtils : UtilsRegistryType
{
    private readonly Dictionary<int, int> battlePetRemap = new Dictionary<int, int>() 
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

        { 2619, 21 }, //Seraph
    };

    internal ClassType FromClassID(int classID)
    {
        if (classID == 28) return ClassType.Healing;
        if (classID == 26 || classID == 27) return ClassType.Summoning;
        return ClassType.Invalid;
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
    Healing
}
