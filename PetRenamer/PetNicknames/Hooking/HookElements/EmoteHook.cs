using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class EmoteHook : HookableElement, IEmoteHook
{
    public delegate BattleChara* OnEmoteDelegate(IntPtr a1, int a2);
    [Signature("33 C0 4C 8D 41 50", DetourName = nameof(OnEmote))]
    readonly Hook<OnEmoteDelegate> hookEmote = null!;

    public BattleChara* LastEmoteUser { get; private set; }

    public EmoteHook(DalamudServices services, IPettableUserList userList, IPetServices petServices) : base(services, userList, petServices)
    {
    }

    public override void Init()
    {
        hookEmote.Enable();
    }

    BattleChara* OnEmote(IntPtr a1, int a2)
    {
        LastEmoteUser = hookEmote.Original(a1, a2);
        return LastEmoteUser;
    }

    public override void Dispose()
    {
        hookEmote?.Dispose();
    }
}
