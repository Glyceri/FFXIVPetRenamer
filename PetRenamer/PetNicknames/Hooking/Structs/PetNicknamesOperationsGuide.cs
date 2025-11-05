using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Runtime.InteropServices;

namespace PetRenamer.PetNicknames.Hooking.Structs;

[StructLayout(LayoutKind.Explicit, Size = 48)]
internal struct PetNicknamesOperationsGuide
{
    [FieldOffset(0)]
    public unsafe AtkStage* AtkStage;

    [FieldOffset(8)]
    public unsafe AtkUnitBase* UnkUnitBase1;

    [FieldOffset(16)]
    public unsafe AtkUnitBase* UnkUnitBase2;

    [FieldOffset(24)]
    private byte Unk18;

    [FieldOffset(25)]
    public byte Unk19;

    [FieldOffset(26)]
    private byte Unk1A;

    [FieldOffset(27)]
    private byte Unk1B;

    [FieldOffset(28)]
    private short X;

    [FieldOffset(30)]
    private short Y;

    [FieldOffset(32)]
    private short Width;

    [FieldOffset(34)]
    private short Height;

    [FieldOffset(36)]
    private float ScaleX;

    [FieldOffset(40)]
    private float Scale;
}
