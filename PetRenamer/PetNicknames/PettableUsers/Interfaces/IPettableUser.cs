using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IPettableUser : IBattleUser, IDisposable
{
    public bool IsActive        { get; }
    public bool IsLocalPlayer   { get; }

    public IPettableDatabaseEntry DataBaseEntry { get; }
    public List<IPettablePet> PettablePets { get; }

    public IPettablePet? GetPet(nint pet);
    public IPettablePet? GetPet(GameObjectId gameObjectId);
    public IPettablePet? GetYoungestPet(PetFilter filter = PetFilter.None);

    public string? GetCustomName(IPetSheetData sheetData);

    public void OnLastCastChanged(uint cast);
    public void Update();
    public void SetBattlePet(BattleChara* battlePet);
    public void RemoveBattlePet(BattleChara* battlePet);
    public void SetCompanion(Companion* companion);
    public void RemoveCompanion(Companion* companion);
    public void RefreshCast();

    public IPettableUserTargetManager? TargetManager { get; }

    public enum PetFilter
    {
        None,
        Minion,
        BattlePet,
        Chocobo
    }
}
