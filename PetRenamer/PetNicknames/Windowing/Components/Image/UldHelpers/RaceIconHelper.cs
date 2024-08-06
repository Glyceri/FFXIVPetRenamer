using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Windowing.Components.Texture;

namespace PetRenamer.PetNicknames.Windowing.Components.Image.UldHelpers;

internal static class RaceIconHelper
{

    public static UldIcon? TierIcon { get; private set; }
    public static UldIcon? ApparatIcon { get; private set; }
    public static UldIcon? PuppeIcon { get; private set; }
    public static UldIcon? MonsterIcon { get; private set; }

    public static void Constructor(in DalamudServices dalamudServices)
    {
        TierIcon = TextureLoader.LoadUld(in dalamudServices, "ui/uld/LovmActionDetail.uld", 3, 0);
        ApparatIcon = TextureLoader.LoadUld(in dalamudServices, "ui/uld/LovmActionDetail.uld", 3, 1);
        PuppeIcon = TextureLoader.LoadUld(in dalamudServices, "ui/uld/LovmActionDetail.uld", 3, 2);
        MonsterIcon = TextureLoader.LoadUld(in dalamudServices, "ui/uld/LovmActionDetail.uld", 3, 3);
    }

    public static UldIcon? GetFromRaceID(uint raceID)
    {
        if (raceID == 1) return TierIcon;
        if (raceID == 2) return MonsterIcon;
        if (raceID == 3) return PuppeIcon;
        if (raceID == 4) return ApparatIcon;

        return null;
    }

    public static void Dispose()
    {
        TierIcon?.Dispose();
        ApparatIcon?.Dispose();
        MonsterIcon?.Dispose();
        PuppeIcon?.Dispose();
    }
}
