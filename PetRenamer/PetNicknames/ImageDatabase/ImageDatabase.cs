using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.ImageDatabase.Interfaces;
using PetRenamer.PetNicknames.ImageDatabase.Texture;
using PetRenamer.PetNicknames.ImageDatabase.Workers;
using PetRenamer.PetNicknames.Lodestone.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;
using PetUser = (string, ushort);

namespace PetRenamer.PetNicknames.ImageDatabase;

internal class ImageDatabase : IImageDatabase
{
    public bool IsDirty { get; private set; }

    readonly List<IGlyceriTextureWrap> _imageDatabase = new List<IGlyceriTextureWrap>();

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

            int length = _imageDatabase.Count;
            for (int i = 0; i < length; i++)
            {
                IGlyceriTextureWrap wrap = _imageDatabase[i];
                if (wrap.User == petUser)
                {
                    wrap.Refresh();
                    return wrap.TextureWrap ?? SearchTexture;
                }
            }

            _imageDatabase.Add(new GlyceriTextureWrap(petUser));
        }
        //TODO: CHECK HERE IF THE USER WANTS TO AUTOMATICALLY DOWNLOAD PICTURES

        ImageDownloader.DownloadImage(databaseEntry, OnSuccess, (e) => PetServices.PetLog.LogException(e));

        return SearchTexture;
    }

    public void Redownload(IPettableDatabaseEntry entry, Action<bool>? callback = null)
    {
        lock (_imageDatabase)
        {
            PetUser petUser = (entry.Name, entry.Homeworld);

            int length = _imageDatabase.Count;
            for (int i = length - 1; i >= 0; i--)
            {
                IGlyceriTextureWrap wrap = _imageDatabase[i];
                if (wrap.User != petUser) continue;

                wrap.TextureWrap?.Dispose();
                _imageDatabase.RemoveAt(i);
                break;
            }
        }
        ImageDownloader.RedownloadImage(entry, (entry, wrap) => { OnSuccess(entry, wrap); callback?.Invoke(true); }, (e) => { callback?.Invoke(true); PetServices.PetLog.LogException(e); });
    }

    public void OnSuccess(IPettableDatabaseEntry entry, IDalamudTextureWrap textureWrap)
    {
        lock (_imageDatabase)
        {
            PetUser petUser = (entry.Name, entry.Homeworld);

            int length = _imageDatabase.Count;
            for (int i = 0; i < length; i++)
            {
                IGlyceriTextureWrap wrap = _imageDatabase[i];
                if (wrap.User != petUser) continue;

                wrap.TextureWrap = textureWrap;
                break;
            }
        }
    }

    public void Dispose()
    {
        SearchTexture?.Dispose();
        ImageDownloader?.Dispose();
        foreach (IGlyceriTextureWrap? tWrap in _imageDatabase)
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

    public void Update()
    {

        int length = _imageDatabase.Count;
        for (int i = length - 1; i >= 0; i--)
        {
            IGlyceriTextureWrap wrap = _imageDatabase[i];
            wrap.Update();
            if (!wrap.IsOld) continue;

            wrap.Dispose();
            _imageDatabase.RemoveAt(i);
        }
    }
}
