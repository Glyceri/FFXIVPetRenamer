using Dalamud.Plugin;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Update;

namespace PetRenamer;

public sealed class PetRenamerPlugin : IDalamudPlugin
{
    DalamudServices _DalamudServices { get; init; }
    IPetServices _PetServices { get; init; }
    IPettableDatabase PettableDatabase { get; init; }

    UpdateHandler UpdateHandler { get; init; }

    public PetRenamerPlugin(IDalamudPluginInterface dalamud)
    {
        _DalamudServices = DalamudServices.Create(ref dalamud)!;
        _PetServices = new PetServices(_DalamudServices);
        PettableDatabase = new PettableDatabase(_PetServices.Configuration, _PetServices.PetLog);
        UpdateHandler = new UpdateHandler(_DalamudServices, PettableDatabase, _PetServices);
    }

    public void Dispose()
    {
        UpdateHandler?.Dispose();
    }
}
