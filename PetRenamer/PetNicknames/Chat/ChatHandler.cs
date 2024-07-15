using PetRenamer.PetNicknames.Chat.ChatElements;
using PetRenamer.PetNicknames.Chat.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.PettableUsers;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Chat;

internal class ChatHandler : IDisposable
{
    readonly DalamudServices DalamudServices;
    readonly IPetServices PetServices;
    readonly IPettableUserList PettableUserList;
    readonly IEmoteHook EmoteHook;

    public ChatHandler(in DalamudServices dalamudServices, in IPetServices petServices, in IPettableUserList pettableUserList, in IEmoteHook emoteHook)
    {
        DalamudServices = dalamudServices;
        PetServices = petServices;
        PettableUserList = pettableUserList;
        EmoteHook = emoteHook;

        _Register();
    }

    void _Register()
    {
        Register(new PetGlamourChat(in DalamudServices, in PetServices, in PettableUserList));
        Register(new EmoteChatElement(in DalamudServices, in PetServices, in PettableUserList, in EmoteHook));
        Register(new BattleChatElement(in PetServices, in PettableUserList));
    }

    readonly List<IChatElement> _chatElements = new List<IChatElement>(); 

    void Register(IChatElement chatElement)
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
