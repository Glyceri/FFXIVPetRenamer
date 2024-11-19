using PetRenamer.Legacy.LegacyStepper;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services;

internal class PetServices : IPetServices
{
    public IPetLog PetLog { get; init; }
    public Configuration Configuration { get; init; }
    public IPetSheets PetSheets { get; init; }
    public IStringHelper StringHelper { get; init; }
    public IPetCastHelper PetCastHelper { get; init; }
    public IPetActionHelper PetActionHelper { get; init; }

    readonly DalamudServices DalamudServices;

    public PetServices(DalamudServices services) 
    {
        DalamudServices = services;

        PetLog = new PetLogWrapper(services.PluginLog);
        Configuration = services.PetNicknamesPlugin.GetPluginConfig() as Configuration ?? new Configuration();
        StringHelper = new StringHelperWrapper(this);
        PetSheets = new SheetsWrapper(ref services, StringHelper);
        PetCastHelper = new PetCastWrapper();
        PetActionHelper = new PetActionWrapper();

        CheckConfigFailure();
    }

    void CheckConfigFailure()
    {
        if (Configuration.currentSaveFileVersion == Configuration.Version) return;
        _ = new LegacyStepper(Configuration, this);
    }
}
