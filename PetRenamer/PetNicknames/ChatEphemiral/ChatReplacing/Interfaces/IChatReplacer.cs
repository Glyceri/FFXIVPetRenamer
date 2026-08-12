using FFXIVClientStructs.FFXIV.Client.System.String;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatReplacing.Interfaces;

internal unsafe interface IChatReplacer
{
    byte[]? Replace(Utf8String* message, int index);
}