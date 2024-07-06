using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class IconNode : Node
{
    public IconNode(uint iconId)
    {
        Style = new Style()
        {
            Size = new Size(64, 64),
            IconId = iconId,
        };
    }
}
