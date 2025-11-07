using KamiToolKit.Nodes;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StyledNodes;

internal class GlowyBarNode : SimpleNineGridNode
{
    public GlowyBarNode()
    {
        TexturePath        = "ui/uld/LovmPaletteEdit.tex";
        TextureCoordinates = Vector2.Zero;
        TextureSize        = new Vector2(54, 22);
        IsVisible          = true;
        Height             = 22;
        Width              = 54;
    }
}
