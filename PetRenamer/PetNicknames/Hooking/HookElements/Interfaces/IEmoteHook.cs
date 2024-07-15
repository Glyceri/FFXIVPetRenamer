using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Hooking.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;

internal unsafe interface IEmoteHook : IHookableElement
{
    public BattleChara* LastEmoteUser {  get; }
}
