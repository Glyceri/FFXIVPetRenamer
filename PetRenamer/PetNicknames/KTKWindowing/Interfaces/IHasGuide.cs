using PetRenamer.PetNicknames.KTKWindowing.Helpers;

namespace PetRenamer.PetNicknames.KTKWindowing.Interfaces;

internal interface IHasGuide
{
    public GuideRegistration GuideRegistration 
        { get; }

    public bool RequestRefresh
        { get; set; }
}
