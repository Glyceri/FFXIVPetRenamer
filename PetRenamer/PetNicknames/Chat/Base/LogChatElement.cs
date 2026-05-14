using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using PetRenamer.PetNicknames.Chat.Interfaces;
using PetRenamer.PetNicknames.Services;
using System;
using System.Collections.Generic;

namespace PetRenamer.PetNicknames.Chat.Base;

internal abstract class LogChatElement : IChatElement, IDisposable
{
    protected readonly DalamudServices DalamudServices;
    
    private readonly HashSet<LogChatMessage> chatMessages = [];
    
    private int expectedLogCount = 0;
    
    protected LogChatElement(DalamudServices dalamudServices)
    {
        DalamudServices = dalamudServices;
        
        DalamudServices.ChatGui.LogMessage += OnLogMessage;
    }
    
    public void Dispose()
    {
        DalamudServices.ChatGui.LogMessage -= OnLogMessage;
    }
    
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
        
        bool hasFoundValidChat = false;
        
        foreach (LogChatMessage logMessage in chatMessages)
        {
            if (logMessage.LogKind != chatMessage.LogKind || logMessage.SourceKind != chatMessage.SourceKind)
            {
                continue;
            }
            
            hasFoundValidChat = true;
            
            break;
        }
        
        if (!hasFoundValidChat)
        {
            return;
        }
        
        expectedLogCount--;
        
        OnValidChatMessage(chatMessage);
    }
    
    protected abstract void OnValidChatMessage(IHandleableChatMessage chatMessage);
    
    protected struct LogChatMessage(uint logId, XivChatType logKind, XivChatRelationKind sourceKind)
    {
        public readonly uint                LogId      = logId;
        public readonly XivChatRelationKind SourceKind = sourceKind;
        public readonly XivChatType         LogKind    = logKind;
    }
}