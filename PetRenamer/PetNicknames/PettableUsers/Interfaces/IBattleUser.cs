using PetRenamer.PetNicknames.PettableUsers.Structs;

namespace PetRenamer.PetNicknames.PettableUsers.Interfaces;

internal interface IBattleUser : IPettableBattleEntity
{
    ActionData CurrentAction { get; }
    
    void OnLastCastChanged(ActionData actionData);
    void Update();
}
