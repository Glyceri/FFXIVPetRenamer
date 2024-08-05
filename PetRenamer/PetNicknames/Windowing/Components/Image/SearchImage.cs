using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.Services;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Image;

internal static class SearchImage
{
    static ISharedImmediateTexture? SearchTexture;

    public static IDalamudTextureWrap? SearchTextureWrap => SearchTexture?.GetWrapOrEmpty();

    public static void Constructor(in DalamudServices dalamudServices)
    {
        SearchTexture = dalamudServices.TextureProvider.GetFromGameIcon(66310);
    }

    public static void Draw(Vector2 size)
    {
        if (SearchTexture == null) return;

        IconImage.Draw(SearchTexture.GetWrapOrEmpty(), size);
    }
}
