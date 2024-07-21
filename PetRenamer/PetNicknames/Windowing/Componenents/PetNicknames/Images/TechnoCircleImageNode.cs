using PetRenamer.PetNicknames.Services;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Images;

internal class TechnoCircleImageNode : RotatableUVImage
{
    const float topLeftX = 0.128f;
    const float topLeftY = 0.58f;

    const float botRightX = 0.328f;
    const float botRightY = 0.937f;

    public TechnoCircleImageNode(in DalamudServices dalamudServices, in Configuration configuration) : base(in dalamudServices, in configuration, 195007, topLeftX, topLeftY, botRightX, botRightY)
    {
        
    }
}
