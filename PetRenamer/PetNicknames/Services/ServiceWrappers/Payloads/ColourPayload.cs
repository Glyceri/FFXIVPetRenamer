using System;
using System.Numerics;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Payloads;

public class ColourPayload : AbstractColourPayload
{
    protected override byte ChunkType => 0x13;

    public ColourPayload(Vector3 color)
    {
        Red = Math.Max((byte)1, (byte)(color.X * 255f));
        Green = Math.Max((byte)1, (byte)(color.Y * 255f));
        Blue = Math.Max((byte)1, (byte)(color.Z * 255f));
    }
}
