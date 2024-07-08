using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class BackgroundNode : Node
{
    public BackgroundNode(uint iconID)
    {
        Style.BackgroundImage = iconID;
        Style.BackgroundImageScale = new Vector2(1, 0.75f);
        Style.BackgroundImageInset = new EdgeSize(4);
        Style.BackgroundImageRotation = 90;
        Style.BackgroundImageBlendMode = BlendMode.Modulate;
        Style.BackgroundColor = new(100, 0, 0, 2);
    }
}
