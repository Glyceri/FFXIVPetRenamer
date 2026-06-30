using Dalamud.Game.Chat;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using PetRenamer.PetNicknames.Chat.ChatElements;
using PetRenamer.PetNicknames.Chat.Interfaces;
using PetRenamer.PetNicknames.Hooking.HookElements.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Chat;

internal class ChatHandler : IDisposable
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
        DalamudServices.ChatGui.LogMessage += OnChat;
        DalamudServices.ChatGui.ChatMessage += OnChat2;
        
        Register(new EmoteChatElement(PetServices));
        Register(new BattleChatElement(PetServices));
        Register(new DebugChatCode(PetServices));
        Register(new SystemChatElement(DalamudServices, PetServices, PronounHook));
    }

    private void Register(IChatElement chatElement)
    {
        _chatElements.Add(chatElement);
        
        DalamudServices.ChatGui.ChatMessage += chatElement.OnChatMessage;
    }
    
    private unsafe void OnChat(ILogMessage chatMessage)
    {
        var start = *(int*)((nint)RaptureLogModule.Instance() + 0x18);
        var count = RaptureLogModule.Instance()->LogMessageCount - start;
        
        PetServices.PetLog.DevLogWarning("CHAT MESSAGE: " + count + ", " + chatMessage.LogMessageId + ", "  + chatMessage.GameData.Value.LogKind.RowId + ", " + chatMessage.SourceEntity?.Name + ", " + chatMessage.TargetEntity?.Name);
    }
    
    private unsafe void OnChat2(IHandleableChatMessage chatMessage)
    {
        var start = *(int*)((nint)RaptureLogModule.Instance() + 0x18);
        var count = RaptureLogModule.Instance()->LogMessageCount - start;
        
        PetServices.PetLog.DevLogWarning("CHAT MESSAGE: " + count + ", " + chatMessage.Sender.TextValue);
    }

    public void Dispose()
    {
        DalamudServices.ChatGui.LogMessage -= OnChat;
        DalamudServices.ChatGui.ChatMessage -= OnChat2;
        
        foreach(IChatElement chatElement in _chatElements)
        {
            if (chatElement is IDisposable disposable)
            {
                disposable.Dispose();
            }
            
            DalamudServices.ChatGui.ChatMessage -= chatElement.OnChatMessage;
        }
    }
}
