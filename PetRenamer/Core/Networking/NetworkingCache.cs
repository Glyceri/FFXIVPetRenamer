using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.Core.Attributes;
using PetRenamer.Core.PettableUserSystem;
using System;
using System.Collections.Generic;

namespace PetRenamer.Core.Networking;

// This is a very dumb class where I manually put in lists or dictionaries or w/e to store networking results.
public class NetworkingCache : IDisposable, IInitializable
{
    internal Dictionary<(string, uint), IDalamudTextureWrap> textureCache = new Dictionary<(string, uint), IDalamudTextureWrap>();
    internal List<(string, uint)> redownloadedUsers = new List<(string, uint)>();
    internal Dictionary<(string, uint), string> lodestoneID = new Dictionary<(string, uint), string>();

    public void Initialize()
    {

    }

    public void Dispose()
    {
        DisposeTextureCache();
        ClearLodestoneCache();
    }

    public void ClearLodestoneCache() => lodestoneID?.Clear();

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

    public void RemoveID((string, uint) character)
    {
        lock (lodestoneID)
        {
            if (!lodestoneID.ContainsKey(character)) return;
            lodestoneID?.Remove(character);
        }
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

    public void AddRedownloadedUsers((string, uint) character)
    {
        lock (redownloadedUsers)
        {
            if (redownloadedUsers.Contains(character)) return;
            redownloadedUsers.Add(character);
        }
    }

    public void RemoveRedownloadedUsers((string, uint) character)
    {
        lock (redownloadedUsers)
        {
            redownloadedUsers?.Remove(character);
        }
    }

    public bool HasRedownloadedUser(PettableUser user)
    {
        lock (redownloadedUsers)
        {
            foreach((string, uint) u in redownloadedUsers)
            {
                if (u.Item1 == user.UserName && u.Item2 == user.Homeworld) return true;
            }
        }
        return false;
    }
}
