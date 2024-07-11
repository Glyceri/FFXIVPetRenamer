using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.ImageDatabase.Workers;
using PetRenamer.PetNicknames.Lodestone.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.ImageDatabase;

internal class ImageDatabase : IImageDatabase
{
    public bool IsDirty { get; private set; }

    readonly Dictionary<ulong, IDalamudTextureWrap?> _imageDatabase = new Dictionary<ulong, IDalamudTextureWrap?>();

    readonly DalamudServices DalamudServices;
    readonly IPetServices PetServices;
    readonly IDalamudTextureWrap SearchTexture;
    readonly ILodestoneNetworker Networker;
    readonly IImageDownloader ImageDownloader;

    public ImageDatabase(in DalamudServices dalamudServices, in IPetServices petServices, in ILodestoneNetworker networker)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        Networker = networker;
        ImageDownloader = new ImageDownloader(DalamudServices, PetServices, Networker);
        SearchTexture = DalamudServices.TextureProvider.GetFromGameIcon(66310).RentAsync().Result;
    }

    public IDalamudTextureWrap? GetWrapFor(IPettableDatabaseEntry? databaseEntry)
    {
        if (databaseEntry == null) return SearchTexture;

        lock (_imageDatabase)
        {
            if (!databaseEntry.IsActive) return SearchTexture;

            ulong contentID = databaseEntry.ContentID;

            if (_imageDatabase.TryGetValue(contentID, out IDalamudTextureWrap? textureWrap))
            {
                if (textureWrap == null) return SearchTexture;
                return textureWrap;
            }

            // CHECK HERE IF THE USER WANTS TO AUTOMATICALLY DOWNLOAD PICTURES

            _imageDatabase.Add(contentID, null);
            ImageDownloader.DownloadImage(databaseEntry, OnSuccess, (e) => PetServices.PetLog.LogException(e));

            return SearchTexture;
        }
    }

    public void Redownload(IPettableDatabaseEntry entry, Action<bool>? callback = null)
    {
        lock (_imageDatabase)
        {
            if(_imageDatabase.TryGetValue(entry.ContentID, out IDalamudTextureWrap? textureWrap))
            {
                if (textureWrap == null) return;
                textureWrap.Dispose();
                _imageDatabase.Remove(entry.ContentID);
            }
            else
            {
                return;
            } 
        }
        ImageDownloader.RedownloadImage(entry, (entry,  wrap) => { OnSuccess(entry, wrap); callback?.Invoke(true); }, (e) => { callback?.Invoke(true); PetServices.PetLog.LogException(e); });
    }

    public void OnSuccess(IPettableDatabaseEntry entry, IDalamudTextureWrap textureWrap)
    {
        lock (_imageDatabase)
        {
            foreach (ulong keyEntry in _imageDatabase.Keys)
            {
                if (keyEntry != entry.ContentID) continue;
                IDalamudTextureWrap? tWrap = _imageDatabase[keyEntry];
                if (tWrap != null) tWrap?.Dispose();
                _imageDatabase[keyEntry] = textureWrap;
                break;
            }
        }
    }

    public void Dispose()
    {
        SearchTexture?.Dispose();
        ImageDownloader?.Dispose();
        foreach (ulong keyEntry in _imageDatabase.Keys)
        {
            IDalamudTextureWrap? tWrap = _imageDatabase[keyEntry];
            if (tWrap == null) continue;
            tWrap?.Dispose();   
        }
    }

    public bool IsBeingDownloaded(IPettableDatabaseEntry? databaseEntry)
    {
        if (databaseEntry == null) return true;
        return ImageDownloader.IsBeingDownloaded(databaseEntry);
    }
}
