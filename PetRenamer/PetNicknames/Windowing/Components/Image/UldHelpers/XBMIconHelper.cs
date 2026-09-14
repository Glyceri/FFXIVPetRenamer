using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using PetRenamer.PetNicknames.Services;

namespace PetRenamer.PetNicknames.Windowing.Components.Image.UldHelpers;

internal static class XBMIconHelper
{
    public static IDalamudTextureWrap?  TopRight          { get; private set; }
    public static IDalamudTextureWrap?  BottomRight       { get; private set; }
    public static IDalamudTextureWrap?  BottomLeft        { get; private set; }
    public static IDalamudTextureWrap?  TopLeft           { get; private set; }
    public static IDalamudTextureWrap?  ActionBlock       { get; private set; }
    public static IDalamudTextureWrap?  BeastActionBlock  { get; private set; }
    public static IDalamudTextureWrap?  BlobImagePart     { get; private set; }
    public static IDalamudTextureWrap?  ActionBacker      { get; private set; }
    
    private static IDalamudTextureWrap? Horn1Texture;
    private static IDalamudTextureWrap? Horn2Texture;
    private static IDalamudTextureWrap? Horn3Texture;
    
    private static UldWrapper?          XBMNoteBookWrapper;
    private static UldWrapper?          XBMNoteDetailWrapper;
    
    public static void Constructor(DalamudServices dalamudServices)
    {
        XBMNoteDetailWrapper = dalamudServices.DalamudPlugin.UiBuilder.LoadUld("ui/uld/XBMMonsterBookDetail.uld");
        XBMNoteBookWrapper   = dalamudServices.DalamudPlugin.UiBuilder.LoadUld("ui/uld/XBMActivePet.uld");
        
        TopLeft             = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/XBMNoteDetail_hr1.tex", 0);
        TopRight            = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/XBMNoteDetail_hr1.tex", 1);
        BottomLeft          = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/XBMNoteDetail_hr1.tex", 2);
        BottomRight         = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/XBMNoteDetail_hr1.tex", 3);
        
        BlobImagePart       = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/XBMNoteParts_hr1.tex",  1);
        ActionBacker        = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/AozNoteBook_hr1.tex",   8);
        
        ActionBlock         = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/IconA_Frame_hr1.tex",   0);
        BeastActionBlock    = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/IconA_Frame_hr1.tex",   4);
        
        Horn1Texture        = XBMNoteBookWrapper.LoadTexturePart("ui/uld/XBMNoteParts_hr1.tex",  1);
        Horn2Texture        = XBMNoteBookWrapper.LoadTexturePart("ui/uld/XBMNoteParts_hr1.tex",  2);
        Horn3Texture        = XBMNoteBookWrapper.LoadTexturePart("ui/uld/XBMNoteParts_hr1.tex",  3);
    }
    
    public static IDalamudTextureWrap? GetHornTexture(int hornSlot)
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
        XBMNoteDetailWrapper?.Dispose();
    }
}