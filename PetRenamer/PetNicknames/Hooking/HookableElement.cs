using PetRenamer.PetNicknames.Hooking.Interfaces;
using PetRenamer.PetNicknames.Services;

namespace PetRenamer.PetNicknames.Hooking;

internal abstract class HookableElement : IHookableElement
{
    public DalamudServices DalamudServices { get; private set; }

    public HookableElement(DalamudServices services)
    {
        DalamudServices = services;
        DalamudServices.Hooking.InitializeFromAttributes(this);
    }

    public abstract void Init();
    public abstract void Dispose();
}
