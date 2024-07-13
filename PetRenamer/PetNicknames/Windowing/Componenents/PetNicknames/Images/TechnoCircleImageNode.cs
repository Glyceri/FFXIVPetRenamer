using PetRenamer.PetNicknames.Services;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Images;

internal class TechnoCircleImageNode : RotatableUVImage
{
    const float topLeftX = 0.128f;
    const float topLeftY = 0.58f;

    const float botRightX = 0.328f;
    const float botRightY = 0.937f;

    public TechnoCircleImageNode(in DalamudServices dalamudServices) : base(in dalamudServices, 195007, topLeftX, topLeftY, botRightX, botRightY)
    {
        Color = new System.Numerics.Vector3(255, 255, 0);
    }
}
