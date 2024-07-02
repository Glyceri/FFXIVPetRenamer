using Dalamud.Plugin;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Update;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    DalamudServices _DalamudServices { get; init; }
    PetServices _PetServices { get; init; }

    UpdateHandler UpdateHandler { get; init; }

    public PetRenamerPlugin(IDalamudPluginInterface dalamud)
    {
        _DalamudServices = DalamudServices.Create(ref dalamud)!;
        _PetServices = new PetServices(_DalamudServices);

        UpdateHandler = new UpdateHandler(_DalamudServices, _PetServices);
    }

    public void Dispose()
    {
        UpdateHandler?.Dispose();
    }
}
