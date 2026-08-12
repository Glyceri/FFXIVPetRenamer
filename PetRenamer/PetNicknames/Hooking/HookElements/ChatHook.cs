using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.Log;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.Hooking.Structs;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.Hooking.HookElements;

internal unsafe class ChatHook : HookableElement
{
    private delegate nint GetLogMessageRawDelegate(LogModule* logModule, int index, nint unk3);
    private delegate nint ClearLogDelegate(LogModule* logModule);
    
    [Signature("E8 ?? ?? ?? ?? 48 8B F8 48 85 C0 0F 84 ?? ?? ?? ?? 49 8B 9E", DetourName = nameof(GetLogMessageRawDetour))]
    private readonly Hook<GetLogMessageRawDelegate>? GetLogMessageRawHook = null;
    
    [Signature("E8 ?? ?? ?? ?? 49 8B CD E8 ?? ?? ?? ?? 45 84 E4", DetourName = nameof(ClearLogDetour))]
    private readonly Hook<ClearLogDelegate>? ClearLogHook = null;
    
    private readonly Hook<RaptureLogModule.Delegates.FormatLogMessage> FormatLogMessageHook;
    private readonly IEphemaralChatHandler ChatHandler;
    
    private int  _lastIndex         = -1;
    private uint _lastIdentifier    = 0;
    private uint _lastSubIdentifier = 0;
    private bool _myCall            = false;
    
    public ChatHook(DalamudServices services, IPetServices petServices, IEphemaralChatHandler ephemaralChatHandler) 
        : base(services, petServices)
    {
        ChatHandler = ephemaralChatHandler;
        
        FormatLogMessageHook = DalamudServices.Hooking.HookFromAddress<RaptureLogModule.Delegates.FormatLogMessage>(RaptureLogModule.MemberFunctionPointers.FormatLogMessage, FormatLogMessageDetour);
    }

    public override void Init()
    {
        FormatLogMessageHook.Enable();
        GetLogMessageRawHook?.Enable();
        ClearLogHook?.Enable();
        
        DalamudServices.ChatGui.LogMessage  += OnChatLog;
        DalamudServices.ChatGui.ChatMessage += OnChatMessage;
    }

    protected override void OnDispose()
    {
        DalamudServices.ChatGui.LogMessage  -= OnChatLog;
        DalamudServices.ChatGui.ChatMessage -= OnChatMessage;
        
        ClearLogHook?.Dispose();
        FormatLogMessageHook.Dispose();
        GetLogMessageRawHook?.Dispose();
    }
    
    private void OnChatLog(ILogMessage logMessage)
    {
        uint        messageId = GetMessageIdentifier(ChatKind.Log);
        XivChatType chatType  =  (XivChatType)logMessage.GameData.Value.LogKind.RowId;
        
        DebugChat(ChatKind.Log, messageId, chatType);
        
        ChatHandler.OnChatLog(messageId, chatType, logMessage.LogMessageId, logMessage.SourceEntity, logMessage.TargetEntity);
    }
    
    private void OnChatMessage(IHandleableChatMessage chatMessage)
    {
        uint        messageId = GetMessageIdentifier(ChatKind.Chat);
        XivChatType chatType  = chatMessage.LogKind;
        
        DebugChat(ChatKind.Chat, messageId, chatType);
        
        ChatHandler.OnChatMessage(messageId, chatType);
    }
    
    private void DebugChat(ChatKind chatKind, uint messageId, XivChatType chatType)
    {
        if (!PetServices.Configuration.debugModeActive)
        {
            return;
        }
        
        PetServices.PetLog.Log($"ChatDebug: {_lastIndex}, {chatKind}, {messageId}, {chatType}.");
    }

    private uint GetMessageIdentifier(ChatKind chatKind)
    {
        PetNicknamesLogModule* logModule = (PetNicknamesLogModule*)RaptureLogModule.Instance();
        
        int start = logModule->NonLogMessageCount;
        int count = RaptureLogModule.Instance()->LogMessageCount - start;
        
        uint currentValue = (uint)(count - start);
        
        if (_lastIndex == -1)
        {
            _lastIndex = (int)currentValue - 1;
        }
        
        if (currentValue == _lastIdentifier && chatKind == ChatKind.Log)
        {
            _lastSubIdentifier++;
        }
        else
        {
            _lastSubIdentifier = 0;
        }
        
        _lastIdentifier = currentValue;
        
        return (uint)(count - start) + _lastSubIdentifier;
    }
    
    private nint ClearLogDetour(LogModule* logModule)
    {
        PetServices.PetLog.DevLogWarning($"Clear LogModule.");
        
        ChatHandler.OnChatClear();
        
        _lastIndex = -1;
        
        return ClearLogHook!.OriginalDisposeSafe(logModule);
    }
    
    private nint GetLogMessageRawDetour(LogModule* logModule, int index, nint unk3)
    {
        _lastIndex = index;
        
        return GetLogMessageRawHook!.OriginalDisposeSafe(logModule, index, unk3);
    }
    
    private uint FormatLogMessageDetour(RaptureLogModule* thisPtr, uint logKindId, Utf8String* sender, Utf8String* message, int* timestamp, void* a6, Utf8String* a7, int chatTabIndex)
    {
        if (_myCall)
        {
            return FormatLogMessageHook.OriginalDisposeSafe(thisPtr, logKindId, sender, message, timestamp, a6, a7, chatTabIndex);
        }
        
        PetServices.PetLog.DevLog($"Trying to handle LogMessage for index: '{_lastIndex}'.");
        
        byte[]? data = ChatHandler.Replace(message, _lastIndex);
        
        if (data == null)
        {
            return FormatLogMessageHook.OriginalDisposeSafe(thisPtr, logKindId, sender, message, timestamp, a6, a7, chatTabIndex);
        }
        
        using Utf8String finalString = new Utf8String(data);
        
        _myCall = true;
        
        uint returner = FormatLogMessageHook.OriginalDisposeSafe(thisPtr, logKindId, sender, &finalString, timestamp, a6, a7, chatTabIndex);
        
        _myCall = false;
        
        return returner;
    }
    
    enum ChatKind
    {
        Log,
        Chat,
    }
}