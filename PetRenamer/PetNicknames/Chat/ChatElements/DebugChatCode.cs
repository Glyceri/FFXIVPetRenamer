using Dalamud.Game.Chat;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using PetRenamer.PetNicknames.Chat.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Chat.ChatElements;

internal class DebugChatCode : IChatElement
{
    private readonly IPetServices PetServices;

    public DebugChatCode(IPetServices petServices) 
        => PetServices = petServices;

    public void OnChatMessage(IHandleableChatMessage chatMessage)
    {
        if (!PetServices.Configuration.debugShowChatCode || !PetServices.Configuration.debugModeActive)
        {
            return;
        }

        chatMessage.Message.Payloads.Insert(0, new TextPayload($"{(int)chatMessage.LogKind}: "));
    }
}
