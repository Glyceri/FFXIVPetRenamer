using Dalamud.Interface.ImGuiNotification;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface INotificationService
{
    IActiveNotification? ShowNotification(NotificationType notificationType, string title, string notificationContent, int duration);
}
