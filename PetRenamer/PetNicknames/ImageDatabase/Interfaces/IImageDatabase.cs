using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;

namespace PetRenamer.PetNicknames.ImageDatabase.Interfaces;

internal interface IImageDatabase : IDisposable
{
    void Redownload(IPettableDatabaseEntry entry, Action<bool>? callback = null);
    void Cancel(IPettableDatabaseEntry entry);
    IDalamudTextureWrap? GetWrapFor(IPettableDatabaseEntry? databaseEntry);
    bool IsBeingDownloaded(IPettableDatabaseEntry? databaseEntry);

    void Update();
}
