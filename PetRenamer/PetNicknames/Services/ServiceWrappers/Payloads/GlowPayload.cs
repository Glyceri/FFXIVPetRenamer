using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Payloads;

public class GlowPayload : AbstractColourPayload
{
    protected override byte ChunkType => 0x14;

    public GlowPayload(Vector3 color)
    {
        Red = Math.Max((byte)1, (byte)(color.X * 255f));
        Green = Math.Max((byte)1, (byte)(color.Y * 255f));
        Blue = Math.Max((byte)1, (byte)(color.Z * 255f));
    }
}
