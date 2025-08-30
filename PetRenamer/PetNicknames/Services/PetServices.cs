using PetRenamer.Legacy.LegacyStepper;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services;

internal class PetServices : IPetServices
{
    public IPetLog          PetLog            { get; }
    public Configuration    Configuration     { get; }
    public IPetSheets       PetSheets         { get; }
    public IStringHelper    StringHelper      { get; }
    public IPetCastHelper   PetCastHelper     { get; }
    public IPetActionHelper PetActionHelper   { get; }
    public ITargetManager   TargetManager     { get; }
    public IPluginWatcher   PluginWatcher     { get; }

    public PetServices(DalamudServices services, IPettableUserList userList) 
    {
        PetLog          = new PetLogWrapper(services.PluginLog);
        Configuration   = services.DalamudPlugin.GetPluginConfig() as Configuration ?? new Configuration();
        StringHelper    = new StringHelperWrapper(this);
        PetSheets       = new SheetsWrapper(ref services, StringHelper);
        PetCastHelper   = new PetCastWrapper();
        PetActionHelper = new PetActionWrapper();
        TargetManager   = new TargetManagerWrapper(services, userList);
        PluginWatcher   = new PluginWatcher(services);

        CheckConfigFailure();
    }

    private void CheckConfigFailure()
    {
        if (Configuration.currentSaveFileVersion == Configuration.Version)
        {
            return;
        }

        _ = new LegacyStepper(Configuration, this);
    }

    public void Dispose()
    {
        PluginWatcher?.Dispose();
    }
}
