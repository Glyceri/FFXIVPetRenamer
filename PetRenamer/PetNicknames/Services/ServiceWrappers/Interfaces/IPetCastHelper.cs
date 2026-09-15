using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Structs;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface IPetCastHelper
{
    IPettableEntity? LastCastDealer { get; }
    ActionData       LastAction     { get; }

    void SetLatestCast(nint target, nint dealer, ActionData actionData);
}
