using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.String;
using System;

namespace PetRenamer.PetNicknames.Hooking.Unsafe;

internal unsafe class TransientGuideString : IDisposable
{
    private readonly Utf8String* StringPtr;

    public TransientGuideString(SeString seString)
    {
        fixed (byte* str = seString.EncodeWithNullTerminator())
        {
            StringPtr = Utf8String.FromSequence(str);
        }
    }
    
    public nint String
        => (nint)StringPtr->StringPtr.Value;

    public void Dispose()
    {
        IMemorySpace.Free(StringPtr);
    }
}
