using System.Collections.Generic;

namespace PetRenamer.PetNicknames;

public static class PluginConstants
{
    public const int ffxivNameSize = 64;
    public const char forbiddenCharacter = '^';

    public const int BaseSummonerSkeleton = -411;
    public const int BaseScholarSkeleton = -407;
    public const int BaseTitanEgiSkeleton = -416;
    public const int BaseGarudaEgiSkeleton = -417;
    public const int BaseIfritEgiSkeleton = -415;

    // Sheets wrapper explains why the order is like this... it's crucial it stays like this.
    // Soft Mapping is the most hardcoded thing in this plogon :c
    // 0 --> Karfunkel
    // 1 --> Garuda-Egi
    // 2 --> Titan-Egi
    // 3 --> Ifrit-Egi
    // 4 --> Eos

    public static readonly int[] BaseSkeletons = new int[5] { BaseSummonerSkeleton, BaseGarudaEgiSkeleton, BaseTitanEgiSkeleton, BaseIfritEgiSkeleton, BaseScholarSkeleton };
}
