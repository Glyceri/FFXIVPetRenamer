using PetRenamer.Legacy.LegacyStepper;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services;

internal class PetServices : IPetServices
{
    public IPetLog              PetLog              { get; }
    public Configuration        Configuration       { get; }
    public IPetSheets           PetSheets           { get; }
    public IStringHelper        StringHelper        { get; }
    public IPetCastHelper       PetCastHelper       { get; }
    public IPetActionHelper     PetActionHelper     { get; }
    public ITargetManager       TargetManager       { get; }
    public IPluginWatcher       PluginWatcher       { get; }
    public INotificationService NotificationService { get; }
    public INameService         NameService         { get; }
    public IHoverService        HoverService        { get; }

    public PetServices(DalamudServices services, IPettableUserList userList) 
    {
        PetLog              = new PetLogWrapper(services.PluginLog);
        Configuration       = services.DalamudPlugin.GetPluginConfig() as Configuration ?? new Configuration();
        StringHelper        = new StringHelperWrapper(this, userList);
        NameService         = new NameService(StringHelper);
        PetSheets           = new SheetsWrapper(services, StringHelper);
        PetCastHelper       = new PetCastWrapper(userList);
        PetActionHelper     = new PetActionWrapper(userList);
        TargetManager       = new TargetManagerWrapper(services, userList);
        PluginWatcher       = new PluginWatcher(services);
        NotificationService = new NotificationService(services, Configuration);
        HoverService        = new HoverService();

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
