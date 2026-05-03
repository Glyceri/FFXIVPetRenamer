using PetRenamer.PetNicknames.Chat.ChatElements;
using PetRenamer.PetNicknames.Chat.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Chat;

internal class ChatHandler : IDisposable
{
    private readonly DalamudServices    DalamudServices;
    private readonly IPetServices       PetServices;
    private readonly IPettableUserList  PettableUserList;

    private readonly List<IChatElement> _chatElements = [];
    
    public ChatHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList pettableUserList)
    {
        DalamudServices  = dalamudServices;
        PetServices      = petServices;
        PettableUserList = pettableUserList;

        _Register();
    }

    private void _Register()
    {
        Register(new PetGlamourChat(DalamudServices, PetServices, PettableUserList));
        Register(new EmoteChatElement(PetServices, PettableUserList));
        Register(new BattleChatElement(PetServices));
        Register(new DebugChatCode(PetServices.Configuration));
    }

    private void Register(IChatElement chatElement)
    {
        _chatElements.Add(chatElement);
        
        DalamudServices.ChatGui.ChatMessage += chatElement.OnChatMessage;
    }

    public void Dispose()
    {
        foreach(IChatElement chatElement in _chatElements)
        {
            DalamudServices.ChatGui.ChatMessage -= chatElement.OnChatMessage;
        }
    }
}
