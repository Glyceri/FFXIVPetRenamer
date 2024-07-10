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
        Style.BackgroundImageColor = new Color(255, 255, 0, 200);
    }
}
