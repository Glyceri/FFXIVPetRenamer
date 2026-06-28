using PetRenamer.Legacy.LegacyStepper;
using PetRenamer.PetNicknames.PettableDatabase;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services;

internal class PetServices : IPetServices
{
    public string               Version             { get; }
    public IPetLog              PetLog              { get; }
    public Configuration        Configuration       { get; }
    public IUserList            UserList            { get; }
    public IPetSheets           PetSheets           { get; }
    public IStringHelper        StringHelper        { get; }
    public IPetCastHelper       PetCastHelper       { get; }
    public ITargetManager       TargetManager       { get; }
    public IPluginWatcher       PluginWatcher       { get; }
    public INotificationService NotificationService { get; }
    public INameService         NameService         { get; }
    public IHoverService        HoverService        { get; }
    public IDirtyCaller         DirtyCaller         { get; }
    public IDirtyListener       DirtyListener       { get; }
    public IParty               Party               { get; }

    private readonly DirtyHandler DirtyHandler = new DirtyHandler();
    
    public PetServices(DalamudServices services) 
    {
        Version             = services.DalamudPlugin.Manifest.AssemblyVersion.ToString();
        PetLog              = new PetLogWrapper(services.PluginLog);
        Configuration       = services.DalamudPlugin.GetPluginConfig() as Configuration ?? new Configuration();
        UserList            = new UserList();
        StringHelper        = new StringHelperWrapper(this, services, UserList);
        NameService         = new NameService(StringHelper);
        PetSheets           = new SheetsWrapper(services);
        PetCastHelper       = new PetCastWrapper(UserList);
        TargetManager       = new TargetManagerWrapper(services, UserList);
        PluginWatcher       = new PluginWatcher(services);
        NotificationService = new NotificationService(services, Configuration);
        HoverService        = new HoverService();
        DirtyCaller         = DirtyHandler;
        DirtyListener       = DirtyHandler;
        Party               = new PartyService(UserList, services, DirtyListener);
        
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
        Party.Dispose();
        PluginWatcher.Dispose();
    }
}
