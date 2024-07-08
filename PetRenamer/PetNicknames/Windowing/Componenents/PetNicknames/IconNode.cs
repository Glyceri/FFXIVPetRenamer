using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class IconNode : Node
{
    public uint? IconID { get => Style.IconId; set => Style.IconId = value; }

    public IconNode()
    {
        
    }
}
