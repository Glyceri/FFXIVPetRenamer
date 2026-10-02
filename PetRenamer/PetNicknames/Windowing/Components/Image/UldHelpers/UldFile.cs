using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.Services;
using System;

namespace PetRenamer.PetNicknames.Windowing.Components.Image.UldHelpers;

internal class UldFile : IDisposable
{
    private readonly DalamudServices DalamudServices;
    private readonly UldWrapper?     UldWrapper;
    public  readonly bool            HasValidFile;
    
    public UldFile(DalamudServices dalamudServices, string path)
    {
        DalamudServices = dalamudServices;
        HasValidFile    = false;
        UldWrapper      = null;
        
        try
        {   
            UldWrapper = dalamudServices.DalamudPlugin.UiBuilder.LoadUld(path);
            HasValidFile = (UldWrapper != null);
        }
        catch (Exception e)
        {
            dalamudServices.PluginLog.Error(e, "Error whilst creating UldFile.");
        }   
    }

    public IDalamudTextureWrap? LoadTexturePart(string texturePath, int part)
    {
        if (UldWrapper == null)
        {
            return null;
        }

        try
        {
            return UldWrapper.LoadTexturePart(texturePath, part);
        }
        catch (Exception e)
        {
            DalamudServices.PluginLog.Error(e, "Error whilst creating UldTexture.");
        }
        
        return null;
    }
    
    public void Dispose()
    {
        UldWrapper?.Dispose();
    }
}