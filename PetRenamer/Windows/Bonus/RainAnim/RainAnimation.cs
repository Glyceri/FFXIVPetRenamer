namespace PetRenamer.Windows.Bonus.RainAnim;

[ToolbarAnimation("Rain")]
internal class RainAnimation : ToolbarAnimation
{
    internal override void OnInitialize()
    {
        for (int i = 0; i < RainSettings.RAIN_COUNT; i++)
            AddElement(CreateRain());

        for (int i = 0; i < RainSettings.FALLING_RAIN_COUNT; i++)
            AddElement(CreateFaillingRain());
    }

    Rain CreateRain() => new Rain(GetRandomPos(), 
        GetRandomRange(RainSettings.SIZE_RANGE_MIN, RainSettings.SIZE_RANGE_MAX),
        GetRandomRange(RainSettings.TIMER_MIN, RainSettings.TIMER_MAX),
        GetRandomRange(RainSettings.RAIN_MIN, RainSettings.RAIN_MAX),
        GetRandomRange(RainSettings.RAIN_SPEED_MIN, RainSettings.RAIN_SPEED_MAX));

    FallingRain CreateFaillingRain() => new FallingRain(GetRandomPos(),
        GetRandomRange(RainSettings.MIN_FALLING_SIZE, RainSettings.MAX_FALLING_SIZE),
        GetRandomRange(RainSettings.RAIN_FALLING_SPEED_MIN, RainSettings.RAIN_FALLING_SPEED_MAX));
}
