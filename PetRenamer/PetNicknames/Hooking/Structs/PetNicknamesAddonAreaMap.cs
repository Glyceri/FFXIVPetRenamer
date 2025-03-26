using System.Runtime.InteropServices;

namespace PetRenamer.PetNicknames.Hooking.Structs;

[StructLayout(LayoutKind.Explicit, Size = 2016)]
internal struct PetNicknamesAddonAreaMap
{
    [FieldOffset(1864)] public int TooltipHoveredIndex;
}
