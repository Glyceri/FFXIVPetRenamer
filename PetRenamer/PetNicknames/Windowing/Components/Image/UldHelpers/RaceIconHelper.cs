using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.Services;

namespace PetRenamer.PetNicknames.Windowing.Components.Image.UldHelpers;

internal static class RaceIconHelper
{
    private static IDalamudTextureWrap? TierIcon    { get; set; }
    private static IDalamudTextureWrap? ApparatIcon { get; set; }
    private static IDalamudTextureWrap? PuppeIcon   { get; set; }
    private static IDalamudTextureWrap? MonsterIcon { get; set; }

    public static void Constructor(DalamudServices dalamudServices)
    {
        UldWrapper uldWrapper = dalamudServices.DalamudPlugin.UiBuilder.LoadUld("ui/uld/LovmActionDetail.uld");
        
        TierIcon    = uldWrapper.LoadTexturePart("ui/uld/iconVerminion_hr1.tex", 0);
        ApparatIcon = uldWrapper.LoadTexturePart("ui/uld/iconVerminion_hr1.tex", 1);
        PuppeIcon   = uldWrapper.LoadTexturePart("ui/uld/iconVerminion_hr1.tex", 2);
        MonsterIcon = uldWrapper.LoadTexturePart("ui/uld/iconVerminion_hr1.tex", 3);
        
        uldWrapper.Dispose();
    }

    public static IDalamudTextureWrap? GetFromRaceId(uint raceId)
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
