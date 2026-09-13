using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Components.Image;
using PetRenamer.PetNicknames.Windowing.Components.Image.UldHelpers;

namespace PetRenamer.PetNicknames.Windowing.Components;

internal static class ComponentLibrary
{
    public static void Initialise(DalamudServices dalamudServices)
    {
        RaceIconHelper.Constructor(dalamudServices);
        XBMIconHelper.Constructor(dalamudServices);
    }

    public static void Dispose()
    {
        RaceIconHelper.Dispose();
        XBMIconHelper.Dispose();
    }
}
