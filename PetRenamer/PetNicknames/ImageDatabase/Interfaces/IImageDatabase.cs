using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using System;

namespace PetRenamer.PetNicknames.ImageDatabase.Interfaces;

internal interface IImageDatabase : IDisposable
{
    bool IsDirty { get; }

    void Redownload(IPettableDatabaseEntry entry);
    IDalamudTextureWrap? GetWrapFor(IPettableDatabaseEntry databaseEntry);
    void OnSuccess(IPettableDatabaseEntry entry, IDalamudTextureWrap textureWrap);
}
