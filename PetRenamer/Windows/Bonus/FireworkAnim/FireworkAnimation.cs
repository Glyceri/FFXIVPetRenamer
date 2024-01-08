using PetRenamer.Windows.Bonus.RainAnim;
using System.Numerics;

namespace PetRenamer.Windows.Bonus.FireworkAnim;

[ToolbarAnimation("Firework")]
internal class FireworkAnimation : ToolbarAnimation
{
    internal override void OnInitialize()
    {
        for (int i = 0; i < 50; i++)
            AddElement(CreateFaillingRain());
    }

    Firework CreateFaillingRain() => new Firework(new Vector2(GetRandom(), GetRandomRange(0.8f, 1.5f)),
       GetRandomRange(RainSettings.MIN_FALLING_SIZE, RainSettings.MAX_FALLING_SIZE),
       GetRandomRange(0.1f, 0.7f),
       GetRandomRange(10, 16));
}
