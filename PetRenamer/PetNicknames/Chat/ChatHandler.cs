using PetRenamer.PetNicknames.Chat.ChatElements;
using PetRenamer.PetNicknames.Chat.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Chat;

internal class ChatHandler : EnablableHandler, IChatHandler
{
    private readonly DalamudServices    DalamudServices;
    private readonly IPetServices       PetServices;
    private readonly IPronounHook       PronounHook;

    private readonly List<IChatElement> _chatElements = [];
    
    public ChatHandler(DalamudServices dalamudServices, IPetServices petServices, IPronounHook pronounHook)
    {
        DalamudServices   = dalamudServices;
        PetServices       = petServices;
        PronounHook       = pronounHook;
        
        _Register();
    }

    private void _Register()
    {
        Register(new EmoteChatElement(PetServices));
        Register(new BattleChatElement(PetServices));
        Register(new DebugChatCode(PetServices));
        Register(new SystemChatElement(DalamudServices, PetServices, PronounHook));
    }

    public override void OnDispose()
    {
        foreach(IChatElement chatElement in _chatElements)
        {
            if (chatElement is not IDisposable disposable)
            {
                continue;
            }
            
            disposable.Dispose();
        }
    }
    
    private void Register(IChatElement chatElement)
    {
        _chatElements.Add(chatElement);
    }

    public override void OnEnable()
    {
        foreach(IChatElement chatElement in _chatElements)
        {
            DalamudServices.ChatGui.ChatMessage -= chatElement.OnChatMessage;
            DalamudServices.ChatGui.ChatMessage += chatElement.OnChatMessage;
            
            if (chatElement is not IEnablableHandler enablableHandler)
            {
                continue;
            }
            
            enablableHandler.Enable();
        }
    }

    public override void OnDisable()
    {
        foreach(IChatElement chatElement in _chatElements)
        {
            DalamudServices.ChatGui.ChatMessage -= chatElement.OnChatMessage;
            
            if (chatElement is not IEnablableHandler enablableHandler)
            {
                continue;
            }
            
            enablableHandler.Disable();
        }
    }
}
