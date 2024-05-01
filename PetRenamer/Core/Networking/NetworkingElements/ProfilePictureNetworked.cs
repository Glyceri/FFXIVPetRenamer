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
using System.Threading;
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
            if (user == null) return GetSearchingTexture();
            if (PluginLink.Configuration.anonymousMode) return GetSearchingTexture();
            if (user.Homeworld == 9999) return GetSearchingTexture();
            (string, uint) currentUser = (user.UserName, user.Homeworld);
            if (!Cache.textureCache.ContainsKey(currentUser)) return GetSearchingTexture();
            nint returner = Cache.textureCache[currentUser]?.ImGuiHandle ?? nint.Zero;
            if (returner == nint.Zero)
            {
                Cache.RemoveTexture(currentUser);
                returner = GetSearchingTexture();
            }
            return returner;
        }
        catch { return GetSearchingTexture(); }
    }

    nint GetSearchingTexture()
    {
        string iconPath = PluginHandlers.TextureProvider.GetIconPath(201)!;
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
            else if (type == UserDeclareType.Add) Task.Run(() => HandleAsAdd(currentUser, force)).ConfigureAwait(false);
        }
        catch (Exception e) { PetLog.Log(e.Message); }
    }

    public void RequestDownload((string, uint) currentUser) => DownloadPagination(currentUser);

    void HandleAsRemove(ref (string, uint) currentUser)
    {
        Cache.RemoveTexture(currentUser);
    }

    void HandleAsAdd((string, uint) currentUser, bool force)
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
                DownloadPagination(cUser);
            }
        }
        catch { }
    }

    public void DeclareDownload((string, uint) characterData)
    {
        try
        {
            Cache.RemoveTexture(characterData);
        }
        catch { }
        Thread.Sleep(3000); // I need dalamud to clear the texture cache first. Hence this stupid sleep, maybe I'm using the texture cache wrong. But I feel like if I manually dispose a texture and load a different one but at the same path, it should just clear.
        Cache.RemoveRedownloadedUsers(characterData);
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
            lock (Cache.textureCache)
            {
                IDalamudTextureWrap wrap = PluginHandlers.TextureProvider.GetTextureFromFile(info)!;
                if (wrap == null) return;
                Cache.textureCache.Add(characterData, wrap);
            }
        }
        catch { }
    }

    void DownloadPagination((string, uint) characterData)
    {
        Cache.AddRedownloadedUsers(characterData);
        Cache.RemoveTexture(characterData);
        try
        {
            LodestoneCharacterSearchElement.instance.SearchCharacter(
            characterData,
            async (SearchData) => await OnSearchData(SearchData, characterData),
            (exception) => PetLog.LogError(exception, exception.Message));
        }
        catch (Exception e) { PetLog.LogError(e, e.Message); }
    }

    async Task OnSearchData(SearchData data, (string, uint) characterData)
    {
        await Task.Run(() =>
        {
            Cache.AddRedownloadedUsers(characterData);
            NetworkedImageDownloader.instance.AsyncDownload(
                data.imageURL!,
                characterData,
                () => DeclareDownload(characterData),
                (exception) => PetLog.Log(exception.Message));
        });
    }

    internal override void OnDispose()
    {
        client?.Dispose();
    }
}
