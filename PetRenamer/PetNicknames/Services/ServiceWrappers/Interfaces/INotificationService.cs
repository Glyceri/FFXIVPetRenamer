using Dalamud.Interface.ImGuiNotification;

namespace PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

internal interface INotificationService
{
    public IActiveNotification? ShowNotification(NotificationType notificationType, string title);
}
