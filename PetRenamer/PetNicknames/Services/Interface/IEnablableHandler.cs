using PetRenamer.PetNicknames.GroupHandling.Interfaces;

namespace PetRenamer.PetNicknames.Services.Interface;

internal interface IEnablableHandler : IHandler
{
    void OnEnable();
    void OnDisable();
    void SetEnabled(EnabledState enabled);
    void Enable();
    void Disable();
}