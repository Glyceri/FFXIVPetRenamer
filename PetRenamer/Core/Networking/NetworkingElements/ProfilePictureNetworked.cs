using PetRenamer.Core.Networking.Attributes;
using PetRenamer.Core.PettableUserSystem.Enums;
using PetRenamer.Core.PettableUserSystem;
using PetRenamer.Core.Singleton;
using Dalamud.Logging;
using PetRenamer.Core.Handlers;
using System.IO;
using PetRenamer.Utilization.UtilsModule;
using ImGuiScene;
using PetRenamer.Core.Networking.Structs;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System;
using System.Threading;

namespace PetRenamer.Core.Networking.NetworkingElements;

[Networked]
public class ProfilePictureNetworked : NetworkingElement, ISingletonBase<ProfilePictureNetworked>
{
    public static ProfilePictureNetworked instance { get; set; } = null!;

    HttpClient client = new HttpClient();

    public nint GetTexture(PettableUser user)
    {
        if (user == null) return nint.Zero;
        (string, uint) currentUser = (user.UserName, user.Homeworld);
        if (!Cache.textureCache.ContainsKey(currentUser))
        {
            string iconPath = PluginHandlers.TextureProvider.GetIconPath(786)!;
            if (iconPath == null) return nint.Zero;
            TextureWrap textureWrap = PluginHandlers.TextureProvider.GetTextureFromGame(iconPath)!;
            if (textureWrap == null) return nint.Zero;
            return textureWrap.ImGuiHandle;
        }
        return Cache.textureCache[currentUser].ImGuiHandle;
    }

    public void OnDeclare(PettableUser user, UserDeclareType type, bool force)
    {
        (string, uint) currentUser = (user.UserName, user.Homeworld);
        if (type == UserDeclareType.Remove) HandleAsRemove(ref currentUser);
        else if (type == UserDeclareType.Add) HandleAsAdd(ref currentUser, force);
    }

    public void RequestDownload((string, uint) currentUser) => Task.Run(() => DownloadPagination(currentUser));

    void HandleAsRemove(ref (string, uint) currentUser)
    {
        if (!Cache.textureCache.ContainsKey(currentUser)) return;
        Cache.textureCache[currentUser]?.Dispose();
        Cache.textureCache.Remove(currentUser);
        PluginLog.LogVerbose($"User Removed: {currentUser}");
    }

    void HandleAsAdd(ref (string, uint) currentUser, bool force)
    {
        if (Cache.textureCache.ContainsKey(currentUser)) return;
        string path = MakeTexturePath(RemapCharacterData(ref currentUser));
        try
        {
            if (Path.Exists(path))
            {
                PluginLog.LogVerbose("File already exists! Grabbing from cache!");
                DeclareDownload(currentUser);
            }
            else if (PluginLink.Configuration.downloadProfilePictures || force)
            {
                PluginLog.LogVerbose("File doesn't exists! Downloading file!");
                DownloadPagination(currentUser);
            }
        }
        catch(Exception e) { PluginLog.Log(e.Message); }
    }

    void DeclareDownload((string, uint) characterData)
    {
        try
        {
            lock (Cache.textureCache)
            {
                if (Cache.textureCache.ContainsKey(characterData))
                    Cache.textureCache[characterData]?.Dispose();
                Cache.textureCache.Remove(characterData);

                string path = MakeTexturePath(RemapCharacterData(ref characterData));
                FileInfo info = new FileInfo(path);
                if (!info.Exists) return;

                TextureWrap wrap = PluginHandlers.TextureProvider.GetTextureFromFile(info, false)!;
                if (wrap == null) return;
                Cache.textureCache.Add(characterData, wrap);
            }
        }
        catch (Exception e) { PluginLog.Log(e.Message); }
    }

    async void DownloadPagination((string, uint) characterData)
    {
        if (Cache.textureCache.ContainsKey(characterData))
        {
            Cache.textureCache[characterData]?.Dispose();
            Cache.textureCache?.Remove(characterData);
        }
        try
        {
            using HttpResponseMessage response = await client?.GetAsync(GetUrl(RemapCharacterData(ref characterData)))!;
            if (response == null) return;
            response.EnsureSuccessStatusCode();
            using Stream str = response.Content.ReadAsStream();
            using StreamReader reader = new StreamReader(str);
            string lines = reader.ReadToEnd();

            PaginationRoot? paginatedStruct = JsonSerializer.Deserialize<PaginationRoot>(lines);
            if (paginatedStruct == null) return;

            List<Result> pStruct = paginatedStruct.Results;
            if (pStruct == null) return;
            if (pStruct.Count == 0) return;
            if (pStruct[0].Avatar == null) return;

            await Task.Run(() => AsyncDownload(pStruct[0].Avatar.ToString(), characterData));
        }
        catch (Exception e) { PluginLog.Log(e.Message); }
    }

    async void AsyncDownload(string URL, (string, uint) charaData)
    {
        try
        {
            CancellationToken cancellationToken = CancellationToken.None;
            using HttpResponseMessage response = await client.GetAsync(URL);
            response.EnsureSuccessStatusCode();

            // Thank DarkArchon for this code :D

            using FileStream fileStream = File.Create(MakeTexturePath(RemapCharacterData(ref charaData)));
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
        catch (Exception e) { PluginLog.Log(e.Message); return; }

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
