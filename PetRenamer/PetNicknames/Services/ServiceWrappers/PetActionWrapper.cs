using FFXIVClientStructs.FFXIV.Client.Game.Character;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class PetActionWrapper : IPetActionHelper
{
    public unsafe BattleChara* LastUser { get; private set; }
    public bool LastValid { get; private set; }

    public unsafe void SetLatestUser(BattleChara* user, bool valid)
    {
        LastUser = user;
        LastValid = valid;
    }
}
