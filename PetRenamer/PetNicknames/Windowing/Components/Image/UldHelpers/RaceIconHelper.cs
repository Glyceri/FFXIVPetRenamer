using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Components.Texture;

namespace PetRenamer.PetNicknames.Windowing.Components.Image.UldHelpers;

internal static class RaceIconHelper
{
    private static UldIcon? TierIcon    { get; set; }
    private static UldIcon? ApparatIcon { get; set; }
    private static UldIcon? PuppeIcon   { get; set; }
    private static UldIcon? MonsterIcon { get; set; }

    public static void Constructor(in DalamudServices dalamudServices)
    {
        TierIcon    = TextureLoader.LoadUld(in dalamudServices, "ui/uld/LovmActionDetail.uld", 3, 0);
        ApparatIcon = TextureLoader.LoadUld(in dalamudServices, "ui/uld/LovmActionDetail.uld", 3, 1);
        PuppeIcon   = TextureLoader.LoadUld(in dalamudServices, "ui/uld/LovmActionDetail.uld", 3, 2);
        MonsterIcon = TextureLoader.LoadUld(in dalamudServices, "ui/uld/LovmActionDetail.uld", 3, 3);
    }

    public static UldIcon? GetFromRaceId(uint raceId)
    {
        return raceId switch
        {
            1 => TierIcon,
            2 => MonsterIcon,
            3 => PuppeIcon,
            4 => ApparatIcon,
            _ => null
        };
    }

    public static void Dispose()
    {
        TierIcon?.Dispose();
        ApparatIcon?.Dispose();
        MonsterIcon?.Dispose();
        PuppeIcon?.Dispose();
    }
}
