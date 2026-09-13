using Dalamud.Interface.Textures;
using PetRenamer.PetNicknames.Services;

namespace PetRenamer.PetNicknames.Windowing.Components.Image;

internal static class SearchImage
{
    public static ISharedImmediateTexture GetSearchTexture(DalamudServices dalamudServices) 
        => dalamudServices.TextureProvider.GetFromGameIcon(242051);
}
