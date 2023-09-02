using System.Runtime.InteropServices;

namespace PetRenamer.Core.Hooking.Structs;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ActionEffect
{
    public ActionEffectType EffectType;
    public byte Param0;
    public byte Param1;
    public byte Param2;
    public byte Flags1;
    public byte Flags2;
    public ushort Value;
}
