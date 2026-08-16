using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using System.Numerics;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Structs;

internal struct ModeToggleRegistration
{
    public readonly SkeletonType PetMode;
    public readonly Vector4      HoverColour;
    public readonly Vector4      IdleColour;
    public readonly Vector4      ClickColour;
    
    public ModeToggleRegistration(SkeletonType petMode, Vector3 hoverColour, Vector3 idleColour, Vector3 clickColour)
    {
        PetMode     = petMode;
        HoverColour = new Vector4(hoverColour, 1.0f);
        IdleColour  = new Vector4(idleColour,  1.0f);
        ClickColour = new Vector4(clickColour, 1.0f);
    }
}