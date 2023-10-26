using Dalamud.Interface.Internal;
using PetRenamer.Core.Handlers;
using PetRenamer.Core.Networking.Attributes;
using PetRenamer.Core.Networking.Structs;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.PettableUserSystem.Enums;
using PetRenamer.Core.Singleton;
using PetRenamer.Logging;
using PetRenamer.Utilization.UtilsModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PetRenamer.Core.Networking.NetworkingElements;

[Networked]
public class ProfilePictureNetworked : NetworkingElement, ISingletonBase<ProfilePictureNetworked>
{
    public static ProfilePictureNetworked instance { get; set; } = null!;

    readonly HttpClient client = new HttpClient();

    public nint GetTexture(PettableUser user)
    {
        try
        {
            if (user == null) return nint.Zero;
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
        string path = MakeTexturePath(RemapCharacterData(ref currentUser));
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
                string path = MakeTexturePath(RemapCharacterData(ref characterData));
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
            using HttpResponseMessage response = await client?.GetAsync(GetUrl(RemapCharacterData(ref characterData)))!;
            if (response == null) return;
            response.EnsureSuccessStatusCode();
            using Stream str = await response.Content.ReadAsStreamAsync();
            if (str == null) return;
            PaginationRoot? paginatedStruct = await JsonSerializer.DeserializeAsync<PaginationRoot>(str);
            if (paginatedStruct == null) return;

            List<Result> pStruct = paginatedStruct.Results;
            if (pStruct == null) return;
            if (pStruct.Count == 0) return;
            if (pStruct[0].Avatar == null) return;

            await Task.Run(() => AsyncDownload(pStruct[0].Avatar.ToString(), characterData));
        }
        catch { }
    }

    async void AsyncDownload(string URL, (string, uint) charaData)
    {
        try
        {
            CancellationToken cancellationToken = CancellationToken.None;
            using HttpResponseMessage response = await client.GetAsync(URL);
            if (response == null) return;
            response.EnsureSuccessStatusCode();

            // Thank DarkArchon for this code :D

            string path = MakeTexturePath(RemapCharacterData(ref charaData));
            FileStream fileStream = File.Create(path);
            await using (fileStream.ConfigureAwait(false))
            {
                int bufferSize = response.Content.Headers.ContentLength > 1024 * 1024 ? 4096 : 1024;
                byte[] buffer = new byte[bufferSize];

                int bytesRead = 0;
                while ((bytesRead = await (await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false)).ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch { return; }

        DeclareDownload(charaData);
    }

    internal override void OnDispose()
    {
        client?.Dispose();
    }

    string GetUrl((string, string) characterData) => $"https://xivapi.com/character/search?name={characterData.Item1}&server={characterData.Item2}";
    (string, string) RemapCharacterData(ref (string, uint) characterData) => (characterData.Item1, SheetUtils.instance.GetWorldName((ushort)characterData.Item2));
    string MakeTexturePath((string, string) characterData) => Path.Combine(Path.GetTempPath(), $"PetNicknames_{characterData.Item1.Replace(" ", "_")}_{characterData.Item2}.jpg");
}
