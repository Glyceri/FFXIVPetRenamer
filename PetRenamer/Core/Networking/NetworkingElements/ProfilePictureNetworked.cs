using Dalamud.Interface.Internal;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Networking.Attributes;
using PetRenamer.Core.Networking.NetworkingElements.Lodestone.LodestoneElement;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Enums;
using PetRenamer.Core.Singleton;
using PetRenamer.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PetRenamer.Core.Networking.NetworkingElements;

[Networked]
public class ProfilePictureNetworked : NetworkingElement, ISingletonBase<ProfilePictureNetworked>
{
    public static ProfilePictureNetworked instance { get; set; } = null!;

    public nint GetTexture(PettableUser user)
    {
        try
        {
            if (user == null) return nint.Zero;
            if (user.Homeworld == 9999) return GetSearchingTexture();
            (string, uint) currentUser = (user.UserName, user.Homeworld);
            if (!Cache.textureCache.ContainsKey(currentUser)) return GetSearchingTexture();
            nint returner = Cache.textureCache[currentUser]?.ImGuiHandle ?? nint.Zero;
            if (returner == nint.Zero) returner = GetSearchingTexture();
            return returner;
        }
        catch { return GetSearchingTexture(); }
    }

    nint GetSearchingTexture()
    {
        string iconPath = PluginHandlers.TextureProvider.GetIconPath(786)!;
        if (iconPath == null) return nint.Zero;
        IDalamudTextureWrap textureWrap = PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!;
        if (textureWrap == null) return nint.Zero;
        return textureWrap.ImGuiHandle;
    }

    public void OnDeclare(PettableUser user, UserDeclareType type, bool force)
    {
        try
        {
            (string, uint) currentUser = (user.UserName, user.Homeworld);
            if (type == UserDeclareType.Remove) HandleAsRemove(ref currentUser);
            else if (type == UserDeclareType.Add) HandleAsAdd(ref currentUser, force);
        }catch(Exception e) { PetLog.Log(e.Message); }
    }

    public void RequestDownload((string, uint) currentUser) => Task.Run(() => DownloadPagination(currentUser));

    void HandleAsRemove(ref (string, uint) currentUser)
    {
        Cache.RemoveTexture(currentUser);
    }

    void HandleAsAdd(ref (string, uint) currentUser, bool force)
    {
        if (Cache.textureCache.ContainsKey(currentUser)) return;
        string path = NetworkedImageDownloader.instance.MakeTexturePath(NetworkedImageDownloader.instance.RemapCharacterData(ref currentUser));
        try
        {
            if (Path.Exists(path))
            {
                DeclareDownload(currentUser);
            }
            else if (PluginLink.Configuration.downloadProfilePictures || force)
            {
                Cache.RemoveTexture(currentUser);
                (string, uint) cUser = currentUser;
                Task.Run(() => DownloadPagination(cUser));
            }
        }
        catch { }
    }

    void DeclareDownload((string, uint) characterData)
    {
        lock (Cache.textureCache)
        {
            try
            {
                Cache.RemoveTexture(characterData);
            }
            catch { }
            FileInfo info = null!;
            try
            {
                string path = NetworkedImageDownloader.instance.MakeTexturePath(NetworkedImageDownloader.instance.RemapCharacterData(ref characterData));
                if (!Path.Exists(path)) return;
                info = new FileInfo(path);
                if (info == null) return;
                if (!info.Exists) return;
            }
            catch { }
            try
            {
                IDalamudTextureWrap wrap = PluginHandlers.TextureProvider.GetTextureFromFile(info, true)!;
                if (wrap == null) return;
                Cache.textureCache.Add(characterData, wrap);
            }
            catch { }
        }
    }

    async void DownloadPagination((string, uint) characterData)
    {
        Cache.RemoveTexture(characterData);
        try
        {
            LodestoneCharacterSearchElement.instance.SearchCharacter(
            characterData,
            (SearchData) => OnSearchData(SearchData, characterData),
            (exception) => throw exception);
        }
        catch(Exception e) { PetLog.LogError(e, e.Message); }
    } 

    async void OnSearchData(SearchData data, (string, uint) characterData)
    {
        await Task.Run(() => 
        {
            NetworkedImageDownloader.instance.AsyncDownload(
                data.imageURL!, 
                characterData, 
                () => 
                { 
                    DeclareDownload(characterData); 

                },
                (exception) => PetLog.Log(exception.Message));
        });
    }

    internal override void OnDispose()
    {
        client?.Dispose();
    }
}
