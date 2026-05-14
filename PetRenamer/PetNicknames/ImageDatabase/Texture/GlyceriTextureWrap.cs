using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using System;
using PetUser = (string, ushort);

namespace PetRenamer.PetNicknames.ImageDatabase.Texture;

internal class GlyceriTextureWrap : IGlyceriTextureWrap
{
    // After 5 seconds a texture is OLD
    private const float TimeToOld = 5.0f;

    private DateTime timeSinceLastAccess = DateTime.Now;

    public GlyceriTextureWrap(in PetUser user)
    {
        User = user;
    }

    public void Dispose()
    {
        TextureWrap?.Dispose();
        TextureWrap = null;
    }

    public IDalamudTextureWrap? TextureWrap
    {
        get; 
        set
        {
            field?.Dispose();
            field = value;
        }
    }
    
    public bool IsOld 
        { get; private set; }
    
    public PetUser User 
        { get; }
    
    public void Update()
    {
        TimeSpan timeSpan = DateTime.Now - timeSinceLastAccess;
        
        if (timeSpan.TotalSeconds < TimeToOld)
        {
            return;
        }

        IsOld = true;
    }

    public void Refresh()
    {
        timeSinceLastAccess = DateTime.Now;
    }
}
