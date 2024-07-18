using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;

namespace PetRenamer.PetNicknames.ImageDatabase.Interfaces;

internal interface IImageDownloader : IDisposable
{
    void DownloadImage(IPettableDatabaseEntry entry, Action<IPettableDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure, bool comesFromAutomation = false);
    void RedownloadImage(IPettableDatabaseEntry entry, Action<IPettableDatabaseEntry, IDalamudTextureWrap> success, Action<Exception> failure);
    bool IsBeingDownloaded(IPettableDatabaseEntry entry);
}
