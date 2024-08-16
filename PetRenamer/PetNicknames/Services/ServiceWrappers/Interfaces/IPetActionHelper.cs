using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal unsafe interface IPetActionHelper
{
    BattleChara* LastUser { get; }
    bool LastValid { get; }

    void SetLatestUser(BattleChara* user, bool valid);
}
