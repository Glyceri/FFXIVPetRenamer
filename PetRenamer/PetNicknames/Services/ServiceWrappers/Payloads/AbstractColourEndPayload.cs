using Dalamud.Game.Text.SeStringHandling;
using System.IO;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Payloads;

public abstract class AbstractColourEndPayload : Payload
{
    protected override byte[] EncodeImpl()
    {
        return new byte[] { START_BYTE, ChunkType, 0x02, 0xEC, END_BYTE };
    }

    protected override void DecodeImpl(BinaryReader reader, long endOfStream) { }
    public override PayloadType Type => PayloadType.Unknown;

    protected abstract byte ChunkType { get; }
}
