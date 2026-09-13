using Dalamud.Interface.Utility;
using System.Numerics;

namespace PetRenamer.PetNicknames.Windowing.Components;

internal static class ImScaler
{
    public static void CreateScale(Vector2 scale, Vector2 baseScale, Vector2 basePosition, out Vector2 newScale, out Vector2 newPosition)
    {
        newScale    = baseScale;
        newPosition = basePosition;
        
        Vector2 scaler = scale * ImGuiHelpers.GlobalScale;
        
        newPosition += scaler;
        newScale    -= scaler * 2;
    }
    
    public static Vector2 MappedScale(Vector2 baseScale, Vector2 scaleToMap)
        => scaleToMap / baseScale;
}