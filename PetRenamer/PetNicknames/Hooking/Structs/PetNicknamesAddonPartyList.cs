using FFXIVClientStructs.FFXIV.Client.UI;
using System.Runtime.InteropServices;

namespace PetRenamer.PetNicknames.Hooking.Structs;

[StructLayout(LayoutKind.Explicit, Size = 5792)]
public struct PetNicknamesAddonPartyList
{
    [FieldOffset(5176)]
    public AddonPartyList.PartyListMemberStruct Pet;
}