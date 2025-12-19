using Dalamud.Interface.ImGuiNotification;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers;

internal class NotificationService : INotificationService
{
    private readonly DalamudServices DalamudServices;
    private readonly Configuration   Configuration;

    public NotificationService(DalamudServices dalamudServices, Configuration configuration)
    {
        DalamudServices = dalamudServices;
        Configuration   = configuration;
    }

    public IActiveNotification? ShowNotification(NotificationType notificationType, string title)
    {
        Notification notification = new Notification
        {
            Title = title,
            Type  = notificationType,
        };

        DalamudServices.PluginLog.Verbose($"Just created a notification of the type: [{notificationType}] with the text: '{title}'.");

        if (!Configuration.showNotifications)
        {
            DalamudServices.PluginLog.Verbose("This notification has not been shown however.");

            return null;
        }

        return DalamudServices.NotificationManager.AddNotification(notification);
    }
}
