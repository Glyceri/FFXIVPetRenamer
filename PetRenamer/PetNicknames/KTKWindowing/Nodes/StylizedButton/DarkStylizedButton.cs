using PetRenamer.PetNicknames.Services.Interface;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;

internal class DarkStylizedButton : StylizedListButton
{
    public DarkStylizedButton(IPetServices petServices) 
        : base(petServices)
    {
        TexturePath               = "ui/uld/img01/TabButtonA.tex";
        TextureSize               = new Vector2(88, 26);
        EnabledTextureCoordinates = new Vector2(0, 26);
    }
}
