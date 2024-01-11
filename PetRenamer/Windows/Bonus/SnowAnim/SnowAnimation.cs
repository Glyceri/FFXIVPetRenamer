using System.Numerics;

namespace PetRenamer.Windows.Bonus.SnowAnim;

[ToolbarAnimation("Snow")]
internal class SnowAnimation : ToolbarAnimation
{
    internal override void OnInitialize()
    {
        for (int i = 0; i < SnowSettings.SNOW_COUNT; i++)
            AddElement(CreateSnow());
    }

    Snow CreateSnow() => new Snow(GetRandomPos(),
                    GetRandomRange(SnowSettings.SIZE_RANGE_MIN, SnowSettings.SIZE_RANGE_MAX),
                    GetRandomRange(SnowSettings.MIN_SPEED, SnowSettings.MAX_SPEED),
                    new Vector3(SnowSettings.MIN_SNOW_FLICKER, SnowSettings.MAX_SNOW_FLICKER, GetRandom(SnowSettings.MAX_SNOW_FLICKER)));
}
