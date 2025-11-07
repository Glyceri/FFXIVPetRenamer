using KamiToolKit.Nodes;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StyledNodes;

internal class SimpleOutlineNode : SimpleNineGridNode
{
    public SimpleOutlineNode()
    {
        TexturePath        = "ui/uld/LovmPaletteEdit.tex";
        TextureCoordinates = new Vector2(56, 22);
        TextureSize        = new Vector2(16, 16);
        IsVisible          = true;
        Height             = 16;
        Width              = 16;
        LeftOffset         = 6;
        RightOffset        = 6;
        BottomOffset       = 6;
        TopOffset          = 6;
    }
}
