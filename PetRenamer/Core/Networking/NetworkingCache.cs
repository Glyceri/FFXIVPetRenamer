using Dalamud.Interface.Internal;
using ImGuiScene;
using PetRenamer.Core.Attributes;
using System;
using System.Collections.Generic;

namespace PetRenamer.Core.Networking;

// This is a very dumb class where I manually put in lists or dictionaries or w/e to store networking results.
public class NetworkingCache : IDisposable, IInitializable
{
    internal Dictionary<(string, uint), IDalamudTextureWrap> textureCache = new Dictionary<(string, uint), IDalamudTextureWrap>();

    public void Initialize()
    {

    }

    public void Dispose()
    {
        DisposeTextureCache();
    }

    public void ClearTextureCache()
    {
        textureCache?.Clear();
    }

    public void DisposeTextureCache()
    {
        foreach (IDalamudTextureWrap texture in textureCache.Values)
            texture?.Dispose();
        ClearTextureCache();
    }

    public void RemoveTexture((string, uint) character)
    {
        lock (textureCache)
        {
            if (!textureCache.ContainsKey(character)) return;
            textureCache[character]?.Dispose();
            textureCache?.Remove(character);
        }
    }
}
