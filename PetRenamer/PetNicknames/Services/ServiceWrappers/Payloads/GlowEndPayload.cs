namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Payloads;

public class GlowEndPayload : AbstractColourEndPayload
{
    protected override byte ChunkType => 0x14;
}
