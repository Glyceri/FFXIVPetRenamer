using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.ImageDatabase.Workers;
using PetRenamer.PetNicknames.Lodestone.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PetUser = (string, ushort);

namespace PetRenamer.PetNicknames.ImageDatabase;

internal class ImageDatabase : IImageDatabase
{
    public bool IsDirty { get; private set; }

    readonly Dictionary<PetUser, IGlyceriTextureWrap?> _imageDatabase = new Dictionary<PetUser, IGlyceriTextureWrap?>();

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
            PetUser petUser = (databaseEntry.Name, databaseEntry.Homeworld);

            if (_imageDatabase.TryGetValue(petUser, out IGlyceriTextureWrap? textureWrap))
            {
                if (textureWrap != null)
                {
                    textureWrap.Refresh();
                    return textureWrap.TextureWrap;
                }
                return SearchTexture;
            }

            //TODO: CHECK HERE IF THE USER WANTS TO AUTOMATICALLY DOWNLOAD PICTURES

            _imageDatabase.Add(petUser, null);
            ImageDownloader.DownloadImage(databaseEntry, OnSuccess, (e) => PetServices.PetLog.LogException(e));

            return SearchTexture;
        }
    }

    public void Redownload(IPettableDatabaseEntry entry, Action<bool>? callback = null)
    {
        lock (_imageDatabase)
        {
            PetUser petUser = (entry.Name, entry.Homeworld);

            if (_imageDatabase.TryGetValue(petUser, out IGlyceriTextureWrap? textureWrap))
            {
                if (textureWrap == null) return;
                textureWrap.Dispose();
                _imageDatabase.Remove(petUser);
            }
            else
            {
                return;
            } 
        }
        ImageDownloader.RedownloadImage(entry, (entry,  wrap) => { OnSuccess(entry, wrap); callback?.Invoke(true); }, (e) => { callback?.Invoke(true); PetServices.PetLog.LogException(e); });
    }

    public void OnSuccess(IPettableDatabaseEntry entry, IGlyceriTextureWrap textureWrap)
    {
        lock (_imageDatabase)
        {
            foreach (PetUser keyEntry in _imageDatabase.Keys)
            {
                if (keyEntry.Item2 != entry.Homeworld) continue;
                if (keyEntry.Item1 != entry.Name) continue;
                IGlyceriTextureWrap? tWrap = _imageDatabase[keyEntry];
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
        foreach (IGlyceriTextureWrap? tWrap in _imageDatabase.Values)
        {
            tWrap?.Dispose();
        }
        _imageDatabase.Clear();
    }

    public bool IsBeingDownloaded(IPettableDatabaseEntry? databaseEntry)
    {
        if (databaseEntry == null) return true;
        return ImageDownloader.IsBeingDownloaded(databaseEntry);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Update()
    {
        List<PetUser> oldUsers = new List<PetUser>();
        foreach (KeyValuePair<PetUser, IGlyceriTextureWrap?> keyValuePair in _imageDatabase)
        {
            IGlyceriTextureWrap? tWrap = keyValuePair.Value;
            if (tWrap == null) continue;

            tWrap.Update();
            if (!tWrap.IsOld) continue;
            tWrap?.Dispose();
            oldUsers.Add(keyValuePair.Key);
        }

        foreach(PetUser user in oldUsers)
        {
            _imageDatabase.Remove(user);
        }
    }
}
