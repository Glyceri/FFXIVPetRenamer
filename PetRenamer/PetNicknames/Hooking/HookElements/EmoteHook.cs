using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class EmoteHook : HookableElement, IEmoteHook
{
    public delegate void OnEmoteDelegate(ulong _, BattleChara* instigatorAddr, ushort emoteId, ulong targetId, ulong __);
    [Signature("40 53 56 41 54 41 57 48 83 EC ?? 48 8B 02", DetourName = nameof(OnEmote))]
    readonly Hook<OnEmoteDelegate> hookEmote = null!;

    public BattleChara* LastEmoteUser { get; private set; }

    public EmoteHook(DalamudServices services, IPettableUserList userList, IPetServices petServices) : base(services, userList, petServices)
    {
    }

    public override void Init()
    {
        hookEmote.Enable();
    }

    void OnEmote(ulong _, BattleChara* instigatorAddr, ushort emoteId, ulong targetId, ulong __)
    {
        hookEmote.Original(_, instigatorAddr, emoteId, targetId, __);

        LastEmoteUser = instigatorAddr;
    }

    public override void Dispose()
    {
        hookEmote?.Dispose();
    }
}
