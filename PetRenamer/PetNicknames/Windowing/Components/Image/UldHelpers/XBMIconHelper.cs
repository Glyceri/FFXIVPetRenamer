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
    public static IDalamudTextureWrap?  XBMEmpty          { get; private set; }
    public static IDalamudTextureWrap?  MinionIcon        { get; private set; }
    public static IDalamudTextureWrap?  Button            { get; private set; }
    
    private static IDalamudTextureWrap? Horn1Texture;
    private static IDalamudTextureWrap? Horn2Texture;
    private static IDalamudTextureWrap? Horn3Texture;
    
    public static void Constructor(DalamudServices dalamudServices)
    {
        using UldFile XBMNoteDetailWrapper  = new UldFile(dalamudServices, "ui/uld/XBMMonsterBookDetail.uld");
        using UldFile XBMActivePetWrapper   = new UldFile(dalamudServices, "ui/uld/XBMActivePet.uld");
        using UldFile MinionNotebookWrapper = new UldFile(dalamudServices, "ui/uld/MinionNoteBook.uld");
        using UldFile XBMNoteBookWrapper    = new UldFile(dalamudServices, "ui/uld/MJIMinionNoteBook.uld");
        
        TopLeft             = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/XBMNoteDetail.tex",   0);
        TopRight            = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/XBMNoteDetail.tex",   1);
        BottomLeft          = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/XBMNoteDetail.tex",   2);
        BottomRight         = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/XBMNoteDetail.tex",   3);
        
        BlobImagePart       = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/XBMNoteParts.tex",    1);
        ActionBacker        = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/AozNoteBook.tex",     8);
        
        ActionBlock         = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/IconA_Frame.tex",     0);
        BeastActionBlock    = XBMNoteDetailWrapper.LoadTexturePart("ui/uld/IconA_Frame.tex",     4);
        
        Horn1Texture        = XBMActivePetWrapper.LoadTexturePart("ui/uld/XBMNoteParts.tex",     1);
        Horn2Texture        = XBMActivePetWrapper.LoadTexturePart("ui/uld/XBMNoteParts.tex",     2);
        Horn3Texture        = XBMActivePetWrapper.LoadTexturePart("ui/uld/XBMNoteParts.tex",     3);
        
        XBMEmpty            = XBMActivePetWrapper.LoadTexturePart("ui/uld/XBMEmpty.tex",         0); 
        
        Button              = XBMNoteBookWrapper.LoadTexturePart("ui/uld/Mji_Window.tex",        26); 
        
        MinionIcon          = MinionNotebookWrapper.LoadTexturePart("ui/uld/MinionNoteBook.tex", 2);
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
        
        XBMEmpty?.Dispose();
        MinionIcon?.Dispose();
        
        Button?.Dispose();
    }
}