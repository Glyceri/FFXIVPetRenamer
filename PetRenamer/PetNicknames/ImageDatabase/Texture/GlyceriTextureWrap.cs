using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using System;

namespace PetRenamer.PetNicknames.ImageDatabase.Texture;

internal class GlyceriTextureWrap : IGlyceriTextureWrap
{
    // After 5 seconds a texture is OLD
    const float TimeToOld = 5.0f;

    public IDalamudTextureWrap? TextureWrap { get; private set; }
    public bool IsOld { get; private set; } = false;

    DateTime timeSinceLastAccess = DateTime.Now;

    public GlyceriTextureWrap(in IDalamudTextureWrap? textureWrap)
    {
        TextureWrap = textureWrap;
    }

    public void Dispose()
    {
        TextureWrap?.Dispose();
        TextureWrap = null;
    }

    public void Update()
    {
        TimeSpan timeSpan = DateTime.Now - timeSinceLastAccess;
        if (timeSpan.TotalSeconds < TimeToOld) return;

        IsOld = true;
    }

    public void Refresh()
    {
        timeSinceLastAccess = DateTime.Now;
    }
}
