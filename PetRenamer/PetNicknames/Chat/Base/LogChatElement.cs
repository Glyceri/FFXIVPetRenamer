using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.Chat.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Chat.Base;

internal abstract class LogChatElement : EnablableHandler, IChatElement, IEnablableHandler
{
    protected readonly DalamudServices DalamudServices;
    
    private readonly HashSet<LogChatMessage> chatMessages = [];
    
    private int expectedLogCount = 0;
    
    protected LogChatElement(DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;
    }
    
    public sealed override void OnEnable()
    {
        chatMessages.Clear();
        
        DalamudServices.ChatGui.LogMessage -= OnLogMessage;
        DalamudServices.ChatGui.LogMessage += OnLogMessage;
    }
    
    public sealed override void OnDisable()
    {
        chatMessages.Clear();
        
        DalamudServices.ChatGui.LogMessage -= OnLogMessage;
    }
    
    public sealed override void OnDispose()
        { }
    
    protected void Register(LogChatMessage logChatMessage)
        => chatMessages.Add(logChatMessage);
    
    private void OnLogMessage(ILogMessage message)
    {
        bool expects = false;
        
        foreach (LogChatMessage chatMessage in chatMessages)
        {
            if (chatMessage.LogId != message.LogMessageId)
            {
                continue;
            }
        
            expects = true;
            
            break;
        }
        
        if (!expects)
        {
            return;
        }
        
        expectedLogCount++;
    }
    
    public void OnChatMessage(IHandleableChatMessage chatMessage)
    {
        if (expectedLogCount <= 0)
        {
            expectedLogCount = 0;
            
            return;
        }

        foreach (LogChatMessage logMessage in chatMessages)
        {
            if (logMessage.LogKind != chatMessage.LogKind || logMessage.SourceKind != chatMessage.SourceKind)
            {
                continue;
            }
            
            expectedLogCount--;
            
            logMessage.Callback(chatMessage);
            
            break;
        }
    }

    protected struct LogChatMessage(uint logId, XivChatType logKind, XivChatRelationKind sourceKind, Action<IHandleableChatMessage> callback)
    {
        public readonly uint                           LogId      = logId;
        public readonly XivChatRelationKind            SourceKind = sourceKind;
        public readonly XivChatType                    LogKind    = logKind;
        public readonly Action<IHandleableChatMessage> Callback   = callback;
    }
}