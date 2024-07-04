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

    public PetServices(DalamudServices services) 
    {
        PetLog = new PetLogWrapper(services.PluginLog);
        Configuration = services.PetNicknamesPlugin.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialise(services.PetNicknamesPlugin);
        CheckConfigFailure();
        PetSheets = new SheetsWrapper(ref services);
        StringHelper = new StringHelperWrapper();
        PetCastHelper = new PetCastWrapper();
    }

    void CheckConfigFailure()
    {
        if (Configuration.currentSaveFileVersion == Configuration.Version) return;
        LegacyStepper legacyStepper = new LegacyStepper(Configuration);
    }
}
