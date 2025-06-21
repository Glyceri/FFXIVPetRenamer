using PetRenamer.Legacy.LegacyStepper;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services;

internal class PetServices : IPetServices
{
    public IPetLog PetLog                   { get; init; }
    public Configuration Configuration      { get; init; }
    public IPetSheets PetSheets             { get; init; }
    public IStringHelper StringHelper       { get; init; }
    public IPetCastHelper PetCastHelper     { get; init; }
    public IPetActionHelper PetActionHelper { get; init; }
    public ITargetManager TargetManager     { get; init; }

    public PetServices(DalamudServices services, IPettableUserList userList) 
    {
        PetLog          = new PetLogWrapper(services.PluginLog);
        Configuration   = services.DalamudPlugin.GetPluginConfig() as Configuration ?? new Configuration();
        StringHelper    = new StringHelperWrapper(this);
        PetSheets       = new SheetsWrapper(ref services, StringHelper);
        PetCastHelper   = new PetCastWrapper();
        PetActionHelper = new PetActionWrapper();
        TargetManager   = new TargetManagerWrapper(services, userList);

        CheckConfigFailure();
    }

    void CheckConfigFailure()
    {
        if (Configuration.currentSaveFileVersion == Configuration.Version)
        {
            return;
        }

        _ = new LegacyStepper(Configuration, this);
    }
}
