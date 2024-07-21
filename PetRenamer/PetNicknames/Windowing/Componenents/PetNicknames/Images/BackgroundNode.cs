using System.Numerics;
using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Images;

internal class BackgroundNode : Node
{
    public BackgroundNode()
    {
        SetStyle1();
    }

    public void SetStyle1()
    {
        Style.BackgroundImage = 194019u;
        Style.BackgroundImageScale = new Vector2(1, 0.75f);
        Style.BackgroundImageInset = new EdgeSize(4);
        Style.BackgroundImageRotation = 90;
        Style.BackgroundImageBlendMode = BlendMode.Modulate;
        Style.BackgroundImageColor = new Color("BackgroundImageColour");
    }
}
