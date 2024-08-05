using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Components.Image;

namespace PetRenamer.PetNicknames.Windowing.Components;

internal static class ComponentLibrary
{
    public static void Initialise(in DalamudServices dalamudServices)
    {
        SearchImage.Constructor(in dalamudServices);
    }

    public static void Dispose()
    {

    }
}
