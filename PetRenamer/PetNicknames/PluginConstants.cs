namespace PetRenamer.PetNicknames;

public static  class PluginConstants
{
    public const int ffxivNameSize = 64;
    public const char forbiddenCharacter = '^';

    public const int BaseSummonerSkeleton = -411;
    public const int BaseScholarSkeleton = -407;
    public const int BaseTitanEgiSkeleton = -416;
    public const int BaseGarudaEgiSkeleton = -417;
    public const int BaseIfritEgiSkeleton = -415;

    public static readonly int[] BaseSkeletons = new int[5] { BaseSummonerSkeleton, BaseGarudaEgiSkeleton, BaseTitanEgiSkeleton, BaseIfritEgiSkeleton, BaseScholarSkeleton };
}
