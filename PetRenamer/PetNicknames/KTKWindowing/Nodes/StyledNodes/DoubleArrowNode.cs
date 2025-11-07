using KamiToolKit.Nodes;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StyledNodes;

internal class DoubleArrowNode : SimpleNineGridNode
{
    public DoubleArrowNode()
    {
        TexturePath        = "ui/uld/ItemDetail.tex";
        TextureCoordinates = new Vector2(124, 0);
        TextureSize        = new Vector2(32, 54);
        IsVisible          = true;
        Height             = 32;
        Width              = 54;
    }
}
