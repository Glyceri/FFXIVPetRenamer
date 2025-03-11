using Dalamud.Game.Text.SeStringHandling;
using System.IO;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Payloads;

public abstract class AbstractColourPayload : Payload
{
    protected byte Red { get; init; }
    protected byte Green { get; init; }
    protected byte Blue { get; init; }

    protected override byte[] EncodeImpl()
    {
        return new byte[] { START_BYTE, ChunkType, 0x05, 0xF6, Red, Green, Blue, END_BYTE };
    }

    protected override void DecodeImpl(BinaryReader reader, long endOfStream) { }
    public override PayloadType Type => PayloadType.Unknown;

    protected abstract byte ChunkType { get; }
}
