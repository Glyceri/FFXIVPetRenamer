using System.Runtime.InteropServices;

// From: https://github.com/Kouzukii/ffxiv-deathrecap/blob/master/Game/ActionEffectHeader.cs

namespace PetRenamer.Core.Hooking.Structs;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ActionEffectHeader
{
    public uint AnimationTargetId;

    public uint Unknown1;

    public uint ActionId;

    public uint GlobalEffectCounter;

    public float AnimationLockTime;

    public uint Unknown2;

    public ushort HiddenAnimation;

    public ushort Rotation;

    public ushort ActionAnimationId;

    public byte Variation;

    public ActionEffectDisplayType EffectDisplayType;

    public byte Unknown3;

    public byte EffectCount;

    public ushort Unknown4;
}
