using System.Runtime.InteropServices;

namespace PetRenamer.PetNicknames.Hooking.Structs;

[StructLayout(LayoutKind.Explicit, Size = 14592)]
internal struct PetNicknamesAddonNaviMap
{
    [FieldOffset(14896)] public int TooltipHoveredIndex;
}
