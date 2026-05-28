using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;
using System.Collections.Generic;
using System.Numerics;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal unsafe interface IPettableUser : IBattleUser
{
    bool IsActive        { get; }
    bool IsLocalPlayer   { get; }

    IPettableDatabaseEntry DataBaseEntry { get; }
    
    List<IPettablePet> PettablePets { get; }

    IPettablePet? GetPet(nint pet);
    IPettablePet? GetPet(GameObjectId gameObjectId);
    IPettablePet? GetYoungestPet(SkeletonType filter = SkeletonType.None);

    string? GetCustomName(IPetSheetData sheetData);

    void OnLastCastChanged(uint cast);
    void Update();
    void SetBattlePet(BattleChara* battlePet);
    void RemoveBattlePet(BattleChara* battlePet);
    void SetCompanion(Companion* companion);
    void RemoveCompanion();
    void Dispose(IPettableDatabase database);
    void GetDrawColours(IPetSheetData sheetData, Configuration.ColourConfig colourConfig, out Vector3? edgeColour, out Vector3? textColour);
    void Recalculate();
}
