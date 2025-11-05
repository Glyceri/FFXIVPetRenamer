using PetRenamer.PetNicknames.Services.Interface;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace PetRenamer.PetNicknames.KTKWindowing.Nodes.StylizedButton;

internal class LightStylizedButton : StylizedListButton
{
    [SetsRequiredMembers]
    public LightStylizedButton(IPetServices petServices)
        : base(petServices)
    {
        TexturePath                = "ui/uld/MiragePrismPlate2.tex";
        TextureSize                = new Vector2(56, 26);
        TextNode.TextColor         = new Vector4(100.0f / 255.0f, 100.0f / 255.0f, 100.0f / 255.0f, 1);
        DisabledTextureCoordinates = new Vector2(0, 26);
        EnabledTextureCoordinates  = new Vector2(56, 26);
    }
}
