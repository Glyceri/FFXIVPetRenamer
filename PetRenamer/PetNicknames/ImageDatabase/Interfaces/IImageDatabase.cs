using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;

namespace PetRenamer.PetNicknames.ImageDatabase.Interfaces;

internal interface IImageDatabase : IDisposable
{
    bool IsDirty { get; }

    void Redownload(IPettableDatabaseEntry entry, Action<bool>? callback = null);
    IDalamudTextureWrap? GetWrapFor(IPettableDatabaseEntry? databaseEntry);
    bool IsBeingDownloaded(IPettableDatabaseEntry? databaseEntry);
    void OnSuccess(IPettableDatabaseEntry entry, IGlyceriTextureWrap textureWrap);

    void Update();
}
