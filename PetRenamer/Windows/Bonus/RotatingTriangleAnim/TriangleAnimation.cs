namespace PetRenamer.Windows.Bonus.RotatingTriangleAnim;

[ToolbarAnimation("Triangle")]
internal class TriangleAnimation : ToolbarAnimation
{
    internal override void OnInitialize()
    {
        for (int i = 0; i < TriangleSettings.TRIANGLE_COUNT; i++)
            AddElement(CreateSnow());
    }

    Triangle CreateSnow() => new Triangle(GetRandomPos(),
                    GetRandomRange(TriangleSettings.SIZE_RANGE_MIN, TriangleSettings.SIZE_RANGE_MAX),
                    GetRandomRange(TriangleSettings.MIN_SPEED, TriangleSettings.MAX_SPEED),
                    GetRandomRange(0, 180),
                    GetRandomRange(TriangleSettings.MIN_ROTATE_SPEED, TriangleSettings.MAX_ROTATE_SPEED));
}
