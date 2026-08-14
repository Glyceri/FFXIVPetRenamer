using Dalamud.Interface.Textures.TextureWraps;
using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Texture;

internal readonly struct UldIcon : IDisposable
{
    public readonly IDalamudTextureWrap Texture;
    public readonly Vector2             Offset;
    public readonly Vector2             Size;

    public UldIcon(IDalamudTextureWrap texture, Vector2 offset, Vector2 size)
    {
        Texture = texture;
        Offset  = offset;
        Size    = size;
    }

    public void Dispose()
    {
        Texture.Dispose();
    }
}