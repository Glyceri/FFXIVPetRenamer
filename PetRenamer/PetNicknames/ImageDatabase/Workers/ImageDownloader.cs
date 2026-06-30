using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.Lodestone.Interfaces;
using PetRenamer.PetNicknames.Lodestone.Structs;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PetRenamer.PetNicknames.ImageDatabase.Workers;

internal class ImageDownloader : IImageDownloader
{
    private readonly HttpClient          HttpClient;
    private readonly DalamudServices     DalamudServices;
    private readonly ILodestoneNetworker Networker;
    private readonly IPetServices        PetServices;

    private readonly Dictionary<ulong, CancellationTokenSource> cancellationTokes = [];

    public ImageDownloader(DalamudServices dalamudServices, IPetServices petServices, ILodestoneNetworker networker)
    {
        HttpClient      = new HttpClient();
        DalamudServices = dalamudServices;
        PetServices     = petServices;
        Networker       = networker;
    }

    public void Dispose()
    {
        foreach (ulong id in cancellationTokes.Keys)
        {
            CancellationTokenSource source = cancellationTokes[id];
            
            try
            {
                source.Cancel();
                source.Dispose();
            }
            catch(Exception e)
            {  
                PetServices.PetLog.LogException(e); 
            }
        }

        cancellationTokes.Clear();
    }
    
    public void Cancel(IPettableDatabaseEntry entry)
    {
        if (!IsBeingDownloaded(entry)) 
        {
            return;
        }
        
        if (!cancellationTokes.TryGetValue(entry.ContentId, out CancellationTokenSource? tokenSource))
        {
            return;
        }
        
        tokenSource.Cancel();
        tokenSource.Dispose();
        
        _ = cancellationTokes.Remove(entry.ContentId);
    }
    
    public void DownloadImage(IPettableDatabaseEntry entry, Action<IPettableDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure, bool comesFromAutomation = false)
    {
        if (IsBeingDownloaded(entry)) 
        {
            return;
        }

        if (FindFileLocally(entry, success, failure))
        {
            return;
        }

        if (!PetServices.Configuration.downloadProfilePictures && comesFromAutomation)
        {
            return;
        }

        _ = Networker.SearchCharacter(entry, (entry, searchData) =>
        {
            OnSuccess(entry, searchData, success, failure);
        }, 
        (e) =>
        {
            PetServices.PetLog.DevLogException(e);
        });
    }

    private void OnSuccess(IPettableDatabaseEntry entry, LodestoneSearchData searchData, Action<IPettableDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure)
    {
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        
        cancellationTokes.Add(entry.ContentId, tokenSource);
        
        CancellationToken token = tokenSource.Token;
        
        _ = Task.Run(async () => await Download(entry, searchData, (entry, wrap) =>
        {
            _ = cancellationTokes.Remove(entry.ContentId);
            success?.Invoke(entry, wrap);
        },
        (e) =>
        {
            _ = cancellationTokes.Remove(entry.ContentId);
            failure?.Invoke(e);
        }, token), token);
    }

    private async Task Download(IPettableDatabaseEntry entry, LodestoneSearchData searchData, Action<IPettableDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure, CancellationToken cancellationToken)
    {
        try
        {
            string? URL = searchData.imageURL;
            
            if (string.IsNullOrEmpty(URL))
            {
                throw new NullReferenceException(URL);
            }

            string filePath = GetFilePath(entry);

            using HttpResponseMessage response = await HttpClient.GetAsync(URL, cancellationToken);
            
            response.EnsureSuccessStatusCode();

            // Thank DarkArchon for this code :D

            FileStream fileStream = File.Create(filePath);
            
            await using (fileStream.ConfigureAwait(false))
            {
                int    bufferSize = response.Content.Headers.ContentLength > 1024 * 1024 ? 4096 : 1024;
                byte[] buffer     = new byte[bufferSize];

                int bytesRead = 0;
                
                while ((bytesRead = await (await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false)).ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch (Exception e)
        {
            failure?.Invoke(e);
            
            return;
        }

        if (!FindFileLocally(entry, success, failure))
        {
            failure?.Invoke(new FileNotFoundException());
        }
    }

    private bool FindFileLocally(IPettableDatabaseEntry databaseEntry, Action<IPettableDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure)
    {
        string filepath = GetFilePath(databaseEntry);

        try
        {
            if (File.Exists(filepath))
            {
                Task.Run(async () => await ReadImage(databaseEntry, success, failure));
                
                return true;
            }
        }
        catch (Exception e)
        {
            failure?.Invoke(e);
        }

        return false;
    }

    private async Task ReadImage(IPettableDatabaseEntry databaseEntry, Action<IPettableDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure)
    {
        try
        {
            ISharedImmediateTexture texture     = DalamudServices.TextureProvider.GetFromFile(GetFilePath(databaseEntry));
            IDalamudTextureWrap     textureWrap = await texture.RentAsync();
            
            success?.Invoke(databaseEntry, textureWrap);
        }
        catch (Exception e)
        {
            failure.Invoke(e);
        }
    }

    private string GetFilePath(IPettableDatabaseEntry entry) 
        => Path.Combine(Path.GetTempPath(), $"PetNicknames_{entry.Name}_{entry.Homeworld}.jpg");

    public void RedownloadImage(IPettableDatabaseEntry entry, Action<IPettableDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure)
    {
        if (IsBeingDownloaded(entry))
        {
            return;
        }
        
        DoRedownloadImage(entry, success, failure);
    }

    private void DoRedownloadImage(IPettableDatabaseEntry entry, Action<IPettableDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure)
    {
        string filepath = GetFilePath(entry);
        
        try
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            
            DownloadImage(entry, success, failure);
        }
        catch (Exception e)
        {
            failure?.Invoke(e);
        }
    }

    public bool IsBeingDownloaded(IPettableDatabaseEntry entry)
    {
        if (Networker.IsBeingDownloaded(entry))
        {
            return true;
        }
        
        foreach (ulong id in cancellationTokes.Keys)
        {
            if (id != entry.ContentId)
            {
                continue;
            }
            
            return true;
        }
        
        return false;
    }
}
