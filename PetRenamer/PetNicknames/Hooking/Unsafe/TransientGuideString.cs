using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.System.String;

namespace PetRenamer.PetNicknames.Hooking.Unsafe;

internal unsafe class TransientGuideString
{
    private readonly SeString SeString;

    public TransientGuideString(SeString seString)
    {
        SeString = seString;
    }

    private nint CreateString()
    {
        fixed (byte* byteString = SeString.EncodeWithNullTerminator())
        {
            return (nint)Utf8String.FromSequence(byteString)->StringPtr.Value;
        }
    }

    public nint String
        => CreateString();
}
