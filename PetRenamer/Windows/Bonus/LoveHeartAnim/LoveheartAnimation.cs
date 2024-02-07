using System;

namespace PetRenamer.Windows.Bonus.LoveHeartAnim;

[ToolbarAnimation("Love Heart")]
internal class LoveheartAnimation : ToolbarAnimation
{
    internal override void OnInitialize()
    {
        for (int i = 0; i < LoveHeartSettings.HEART_COUNT; i++)
            AddElement(CreateLoveHeart());
    }


    Loveheart CreateLoveHeart() => new Loveheart(GetRandomPos(),
        MathF.Round(GetRandomRange(LoveHeartSettings.SIZE_RANGE_MIN, LoveHeartSettings.SIZE_RANGE_MAX)),
        GetRandomRange(LoveHeartSettings.HEART_SPEED_MIN, LoveHeartSettings.HEART_SPEED_MAX));
}
