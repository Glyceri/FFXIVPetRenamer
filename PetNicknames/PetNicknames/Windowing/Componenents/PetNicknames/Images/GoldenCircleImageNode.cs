
using PetRenamer.PetNicknames.Services;

namespace PetRenamer.PetNicknames.Windowing.Componenents.PetNicknames.Images;

internal class GoldenCircleImageNode : RotatableUVImage
{
    public GoldenCircleImageNode(in DalamudServices dalamudServices, in Configuration configuration) : base(dalamudServices, configuration, 195714, 0, 0.2f, 1, 0.82f)
    {
    }
}
