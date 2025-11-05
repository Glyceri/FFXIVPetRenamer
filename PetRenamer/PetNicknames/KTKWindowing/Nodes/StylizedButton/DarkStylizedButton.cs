using PetRenamer.PetNicknames.Services.Interface;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;

internal class DarkStylizedButton : StylizedListButton
{
    [SetsRequiredMembers]
    public DarkStylizedButton(IPetServices petServices) 
        : base(petServices)
    { 
        TexturePath               = "ui/uld/TabButtonA.tex";
        TextureSize               = new Vector2(88, 26);
        LeftOffset                = 16;
        RightOffset               = 16;
        EnabledTextureCoordinates = new Vector2(0, 26);
    }
}
