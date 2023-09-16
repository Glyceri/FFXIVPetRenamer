using ImGuiScene;
using PetRenamer.Core.Attributes;
using System;
using System.Collections.Generic;

namespace PetRenamer.Core.Networking;

// This is a very dumb class where I manually put in lists or dictionaries or w/e to store networking results.
public class NetworkingCache : IDisposable, IInitializable
{
    internal Dictionary<(string, uint), TextureWrap> textureCache = new Dictionary<(string, uint), TextureWrap>();

    public void Initialize()
    {

    }

    public void Dispose()
    {
        ClearTextureCache();
    }

    public void ClearTextureCache()
    {
        DisposeTextureCache();
        textureCache?.Clear();
    }

    public void DisposeTextureCache()
    {
        foreach (TextureWrap texture in textureCache.Values)
            texture?.Dispose();
    }

    public void RemoveTexture((string, uint) character)
    {
        if (!textureCache.ContainsKey(character)) return;
        textureCache[character]?.Dispose();
        textureCache?.Remove(character);
    }
}
