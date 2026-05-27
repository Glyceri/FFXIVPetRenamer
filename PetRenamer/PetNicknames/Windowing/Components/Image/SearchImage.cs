using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.Services;

namespace PetRenamer.PetNicknames.Windowing.Components.Image;

internal static class SearchImage
{
    private static ISharedImmediateTexture? SearchTexture;

    public static void Constructor(in DalamudServices dalamudServices) 
        => SearchTexture = dalamudServices.TextureProvider.GetFromGameIcon(66310);
    
    public static IDalamudTextureWrap? SearchTextureWrap 
        => SearchTexture?.GetWrapOrEmpty();
}
