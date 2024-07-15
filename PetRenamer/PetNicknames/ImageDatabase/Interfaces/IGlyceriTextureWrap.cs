using Dalamud.Interface.Textures.TextureWraps;
using System;

namespace PetRenamer.PetNicknames.ImageDatabase.Interfaces;

internal interface IGlyceriTextureWrap : IDisposable
{
    IDalamudTextureWrap? TextureWrap { get; }

    bool IsOld { get; }

    void Update();
    void Refresh();
}
