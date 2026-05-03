using Lumina.Excel.Sheets;
using PetRenamer.PetNicknames.Chat.ChatElements;
using PetRenamer.PetNicknames.Chat.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
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
    private readonly IPronounHook       PronounHook;

    private readonly List<IChatElement> _chatElements = [];
    
    public ChatHandler(DalamudServices dalamudServices, IPetServices petServices, IPettableUserList pettableUserList, IPronounHook pronounHook)
    {
        DalamudServices   = dalamudServices;
        PetServices       = petServices;
        PettableUserList  = pettableUserList;
        PronounHook       = pronounHook;
        
        _Register();
    }

    private void _Register()
    {
        Register(new PetGlamourChat(DalamudServices, PetServices, PettableUserList));
        Register(new EmoteChatElement(PetServices, PettableUserList));
        Register(new BattleChatElement(PetServices));
        Register(new DebugChatCode(PetServices.Configuration));
        Register(new SystemChatElement(DalamudServices, PetServices, PronounHook, PettableUserList));
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
