using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.Services;

namespace PetRenamer.PetNicknames.Windowing.Components.Image.UldHelpers;

internal static class XBMIconHelper
{
    public static IDalamudTextureWrap TopRight          { get; private set; } = null!;
    public static IDalamudTextureWrap BottomRight       { get; private set; } = null!;
    public static IDalamudTextureWrap BottomLeft        { get; private set; } = null!;
    public static IDalamudTextureWrap TopLeft           { get; private set; } = null!;
    public static IDalamudTextureWrap ActionBlock       { get; private set; } = null!;
    public static IDalamudTextureWrap BeastActionBlock  { get; private set; } = null!;
    public static IDalamudTextureWrap BlobImagePart     { get; private set; } = null!;
    
    private static IDalamudTextureWrap Horn1Texture = null!;
    private static IDalamudTextureWrap Horn2Texture = null!;
    private static IDalamudTextureWrap Horn3Texture = null!;
    
    private static UldWrapper XBMNoteBookWrapper = null!;
    
    public static void Constructor(DalamudServices dalamudServices)
    {
        XBMNoteBookWrapper  = dalamudServices.DalamudPlugin.UiBuilder.LoadUld("ui/uld/XBMMonsterBookDetail.uld");
        
        TopLeft             = XBMNoteBookWrapper.LoadTexturePart("ui/uld/XBMNoteDetail_hr1.tex", 0)!;
        TopRight            = XBMNoteBookWrapper.LoadTexturePart("ui/uld/XBMNoteDetail_hr1.tex", 1)!;
        BottomLeft          = XBMNoteBookWrapper.LoadTexturePart("ui/uld/XBMNoteDetail_hr1.tex", 2)!;
        BottomRight         = XBMNoteBookWrapper.LoadTexturePart("ui/uld/XBMNoteDetail_hr1.tex", 3)!;
        
        BlobImagePart       = XBMNoteBookWrapper.LoadTexturePart("ui/uld/XBMNoteParts_hr1.tex",  1)!;
        
        ActionBlock         = XBMNoteBookWrapper.LoadTexturePart("ui/uld/IconA_Frame_hr1.tex",   0)!;
        BeastActionBlock    = XBMNoteBookWrapper.LoadTexturePart("ui/uld/IconA_Frame_hr1.tex",   4)!;
        
        Horn1Texture        = XBMNoteBookWrapper.LoadTexturePart("ui/uld/XBMNoteParts_hr1.tex", 2)!;
        Horn2Texture        = XBMNoteBookWrapper.LoadTexturePart("ui/uld/XBMNoteParts_hr1.tex", 3)!;
        Horn3Texture        = XBMNoteBookWrapper.LoadTexturePart("ui/uld/XBMNoteParts_hr1.tex", 4)!;
    }
    
    public static IDalamudTextureWrap? GetHornTexture(sbyte hornSlot)
    {
        switch (hornSlot)
        {
            case 0: return Horn1Texture;
            case 1: return Horn2Texture;
            case 2: return Horn3Texture;
        }
        
        return null;
    }
    
    public static void Dispose()
    {
        TopRight?.Dispose();
        TopLeft?.Dispose();
        BottomLeft?.Dispose();
        BottomRight?.Dispose();
        
        ActionBlock?.Dispose();
        BeastActionBlock?.Dispose();
        
        BlobImagePart?.Dispose();
        
        Horn1Texture?.Dispose();
        Horn2Texture?.Dispose();
        Horn3Texture?.Dispose();
        
        XBMNoteBookWrapper?.Dispose();
    }
}