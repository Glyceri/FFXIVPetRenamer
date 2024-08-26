using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Lumina.Data.Files;
using PetRenamer.PetNicknames.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Texture;

internal struct UldIcon : IDisposable
{
    public IDalamudTextureWrap Texture { get; private set; }
    public Vector2 Offset { get; }
    public Vector2 Size { get; }

    public UldIcon(IDalamudTextureWrap texture, Vector2 offset, Vector2 size)
    {
        Texture = texture;
        Offset = offset;
        Size = size;
    }

    public void Dispose()
    {
        Texture.Dispose();
    }
}


internal static class TextureLoader
{
    static readonly Dictionary<string, TexFile> PathToTexFileCache = [];
    static readonly Dictionary<string, UldFile> PathToUldFileCache = [];

    internal static UldFile? LoadUldFile(in DalamudServices dalamudServices, string path)
    {
        if (!PathToUldFileCache.TryGetValue(path, out UldFile? uldFile))
        {
            uldFile = dalamudServices.DataManager.GetFile<UldFile>(path);
            if (uldFile == null) return null;
            PathToUldFileCache[path] = uldFile;
        }

        
        return uldFile;
    }

    internal static unsafe UldIcon? LoadUld(in DalamudServices dalamudServices, string uldPath, int partsId, int partId)
    {
        if (!uldPath.EndsWith(".uld"))
        {
            if (uldPath.Contains('.'))
                return null;
            uldPath += ".uld";
        }

        UldFile? uldFile = LoadUldFile(in dalamudServices, uldPath);

        if (uldFile == null)
            return null;

        var part = uldFile.Parts.First(t => t.Id == partsId);
        var subPart = part.Parts[partId];
        var tex = uldFile.AssetData.First(t => t.Id == subPart.TextureId).Path;
        string texPath;
        fixed (char* p = tex)
            texPath = new string(p);
        var normalTexPath = texPath;
        var scale = 2;
        texPath = texPath[..^4] + "_hr1.tex";
        var texFile = LoadTexture(in dalamudServices, texPath);
        // failed to get hr version of texture? Fallback to normal
        if (texFile == null)
        {
            scale = 1;
            texFile = LoadTexture(in dalamudServices, normalTexPath);

            if (texFile == null)
                return null;
        }

        var uv = new Vector2(subPart.U, subPart.V) * scale;
        var size = new Vector2(subPart.W, subPart.H) * scale;

        return new UldIcon(texFile, uv, size);
    }

    internal static IDalamudTextureWrap? LoadTexture(in DalamudServices dalamudServices, string path)
    {
        if (!PathToTexFileCache.TryGetValue(path, out TexFile? texFile))
        {
            path = dalamudServices.TextureSubstitutionProvider.GetSubstitutedPath(path);

            texFile = Path.IsPathRooted(path)
                ? dalamudServices.DataManager.GameData.GetFileFromDisk<TexFile>(path)
                : dalamudServices.DataManager.GetFile<TexFile>(path);

            if (null == texFile) return null;

            PathToTexFileCache[path] = texFile;
        }

        

        return dalamudServices.TextureProvider.CreateFromRaw(RawImageSpecification.Bgra32(texFile.Header.Width, texFile.Header.Height), texFile.ImageData.AsSpan());
        
    }
}
