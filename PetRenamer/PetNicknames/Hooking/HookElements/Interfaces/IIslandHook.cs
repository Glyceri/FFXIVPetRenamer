using PetRenamer.PetNicknames.Hooking.Interfaces;

namespace PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;

internal interface IIslandHook : IHookableElement
{
    bool    IsOnIsland          { get; }
    bool    IslandStatusChanged { get; }
    string? VisitingFor         { get; }
    uint?   VisitingHomeworld   { get; }
    
    void Update();
}
