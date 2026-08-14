using System.Runtime.InteropServices;

namespace PetRenamer.PetNicknames.Hooking.Structs;

[StructLayout(LayoutKind.Explicit, Size = 128 /*0x80*/)]
public struct PetNicknamesLogModule
{
    [FieldOffset(20)]
    public int LogMessageCount;
    [FieldOffset(0x18)]
    public int NonLogMessageCount;
}