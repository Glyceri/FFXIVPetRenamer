using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using System;
using PetUser = (string, ushort);

namespace PetRenamer.PetNicknames.ImageDatabase.Texture;

internal class GlyceriTextureWrap : IGlyceriTextureWrap
{
    // After 5 seconds a texture is OLD
    const float TimeToOld = 5.0f;

    IDalamudTextureWrap? textureWrap = null;

    public IDalamudTextureWrap? TextureWrap
    {
        get => textureWrap; 
        set
        {
            textureWrap?.Dispose();
            textureWrap = value;
        }
    }
    public bool IsOld { get; private set; } = false;
    public PetUser User { get; }

    DateTime timeSinceLastAccess = DateTime.Now;

    public GlyceriTextureWrap(in PetUser user)
    {
        User = user;
    }

    public GlyceriTextureWrap(in IDalamudTextureWrap? textureWrap, in PetUser user)
    {
        this.textureWrap = textureWrap;
        User = user;
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
