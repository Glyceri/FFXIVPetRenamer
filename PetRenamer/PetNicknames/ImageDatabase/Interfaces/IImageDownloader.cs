using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;

namespace PetRenamer.PetNicknames.ImageDatabase.Interfaces;

internal interface IImageDownloader : IDisposable
{
    void DownloadImage(IPettableDatabaseEntry entry, Action<IPettableDatabaseEntry, IGlyceriTextureWrap> success, Action<Exception> failure);
    void RedownloadImage(IPettableDatabaseEntry entry, Action<IPettableDatabaseEntry, IGlyceriTextureWrap> success, Action<Exception> failure);
    bool IsBeingDownloaded(IPettableDatabaseEntry entry);
}
