namespace PetRenamer.PetNicknames.Hooking.Enums;

internal enum MapTooltipType : byte
{
    MapMarker              = 2,     // Loops through all MapMarkers and handles those tooltips.
    MapObjectInfoMarker    = 4,     // Loops through all MapObjectInfo's and handles those tooltips.
    BattleCharaMarker      = 5,     // Tooltip for BattleCharas (Calls the GetName function on them and copies that into the tooltip).
    Unk6                   = 6,     // Seems complicated c:
    TempMarker             = 7,     // Handles tooltips for the TempMarker list.
    MiniMapGatherMarker    = 9,     // Handles tooltips for all MiniMapGather markers.
    MiniMapMarker          = 0xA,   // Handles tooltips for the Minimap (different from the area map, even though they often display the same tooltips. Its like almost the same code as 0x2).
    UnkB                   = 0xB,   // It checks the count and loops through an unknown marker list.
    UnkD                   = 0xD,   // Just copies the field 0x58C0 into the tooltip.
}