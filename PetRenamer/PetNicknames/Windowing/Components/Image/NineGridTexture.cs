using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures.TextureWraps;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components.Image;

internal static class NineGridTexture
{ 
    public static void Draw(IDalamudTextureWrap texture, Vector2 position, Vector2 size, int inset, Vector4 colour)
    {
        int width         = texture.Width;
        int height        = texture.Height;

        int doubleInset   = inset * 2;
        
        int middleWidth   = (int)size.X - doubleInset;
        int middleHeight  = (int)size.Y - doubleInset;
        int midOffsetX    = (int)size.X - inset;
        int midOffsetY    = (int)size.Y - inset;
        
        int imgWidthSize  = width - doubleInset;
        int imgWidthPos   = width - inset;
        int imgHeightSize = height - doubleInset;
        int imgHeightPos  = height - inset;
        
        DrawImagePart(texture, position, 0,          0,          inset,       inset,        0,           0,            inset,        inset,         colour);
        DrawImagePart(texture, position, inset,      0,          middleWidth, inset,        inset,       0,            imgWidthSize, inset,         colour);
        DrawImagePart(texture, position, midOffsetX, 0,          inset,       inset,        imgWidthPos, 0,            inset,        inset,         colour);
        
        DrawImagePart(texture, position, 0,          inset,      inset,       middleHeight, 0,           inset,        inset,        imgHeightSize, colour);
        DrawImagePart(texture, position, inset,      inset,      middleWidth, middleHeight, inset,       inset,        imgWidthSize, imgHeightSize, colour);
        DrawImagePart(texture, position, midOffsetX, inset,      inset,       middleHeight, imgWidthPos, inset,        inset,        imgHeightSize, colour);
        
        DrawImagePart(texture, position, 0,          midOffsetY, inset,       inset,        0,           imgHeightPos, inset,        inset,         colour);
        DrawImagePart(texture, position, inset,      midOffsetY, middleWidth, inset,        inset,       imgHeightPos, imgWidthSize, inset,         colour);
        DrawImagePart(texture, position, midOffsetX, midOffsetY, inset,       inset,        imgWidthPos, imgHeightPos, inset,        inset,         colour);
    }
    
    private static void DrawImagePart(
        IDalamudTextureWrap texture, Vector2 basePosition, 
        int destX, int destY, int destW, int destH,
        int srcX, int srcY, int srcW, int srcH, Vector4 colour)
    {
        int width  = texture.Width;
        int height = texture.Height;

        Vector2 positionBase = basePosition + new Vector2(destX, destY);
        Vector2 scaledSize   = new Vector2(destW, destH);

        Vector2 uvMin = new Vector2((float)srcX / width, (float)srcY / height);
        Vector2 uvMax = new Vector2((float)(srcX + srcW) / width, (float)(srcY + srcH) / height);

        ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
        
        windowDrawList.AddImage(texture.Handle, positionBase, positionBase + scaledSize, uvMin, uvMax, ImGui.GetColorU32(colour / new Vector4(255)));
    }
}