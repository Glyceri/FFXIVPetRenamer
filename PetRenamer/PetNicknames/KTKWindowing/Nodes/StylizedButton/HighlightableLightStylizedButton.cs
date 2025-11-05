using PetRenamer.PetNicknames.Services.Interface;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;

internal class HighlightableLightStylizedButton : LightStylizedButton
{
    [SetsRequiredMembers]
    public HighlightableLightStylizedButton(IPetServices petServices)
        : base(petServices)
    {
        Unfocus();
    }

    public void Unfocus()
    {
        ImageNode.Color = new Vector4(0.87f, 0.87f, 0.87f, 1);
    }

    public void Focus()
    {
        ImageNode.Color = new Vector4(1, 1, 1, 1);
    }
}
