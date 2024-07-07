using Una.Drawing;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames;

internal class IconNode : Node
{
    public IconNode(uint iconId)
    {
        Style = new Style()
        {
            IconId = iconId,
            Anchor = Anchor.MiddleCenter,
        };

        BeforeReflow += _ => 
        {
            EdgeSize PaddingSize = new EdgeSize();
            EdgeSize MarginSize = new EdgeSize();
            if (ParentNode!.ComputedStyle.Padding != null) PaddingSize = ParentNode!.ComputedStyle.Padding;
            if (ParentNode!.ComputedStyle.Margin != null) MarginSize = ParentNode!.ComputedStyle.Margin;
            Style.Size = (ParentNode!.Bounds.ContentSize - PaddingSize.Size - MarginSize.Size) / ScaleFactor;
            return true;
        };
    }
}
