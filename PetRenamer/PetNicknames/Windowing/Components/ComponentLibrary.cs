using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Components.Image;
using PetRenamer.PetNicknames.Windowing.Components.Image.UldHelpers;

namespace PetRenamer.PetNicknames.Windowing.Components;

internal static class ComponentLibrary
{
    public static void Initialise(in DalamudServices dalamudServices)
    {
        SearchImage.Constructor(in dalamudServices);
        RaceIconHelper.Constructor(in dalamudServices);
    }

    public static void Dispose()
    {
        RaceIconHelper.Dispose();
    }
}
