using PetRenamer.Core.Singleton;
using PetRenamer.Utilization.Attributes;

namespace PetRenamer.Utilization.UtilsModule;

[UtilsDeclarable]
internal class MathUtils : UtilsRegistryType, ISingletonBase<MathUtils>
{
    public static MathUtils instance { get; set; } = null!;

    internal bool IsInRange(int value, int max, bool inclusive = true) => IsInRange(value, 0, max, inclusive);
    internal bool IsInRange(float value, float max, bool inclusive = true) => IsInRange(value, 0, max, inclusive);
    
    internal bool IsInRange(int value, int min, int max, bool inclusive = true)
    {
        if (inclusive) return value >= min && value <= max;
        return value > min && value < max;
    }

    internal bool IsInRange(float value, float min, float max, bool inclusive = true)
    {
        if (inclusive) return value >= min && value <= max;
        return value > min && value < max;
    }

    internal float Map(float x, float in_min, float in_max, float out_min = 0, float out_max = 1)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
}
