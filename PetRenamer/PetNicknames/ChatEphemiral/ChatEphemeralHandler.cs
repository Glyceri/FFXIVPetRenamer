using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.System.String;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatReplacing;
using PetRenamer.PetNicknames.ChatEphemiral.ChatReplacing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.ChatEphemiral;

internal unsafe class ChatEphemeralHandler : EnablableHandler, IEphemaralChatHandler
{
    private readonly IChatDatabaseHandler ChatDatabaseHandler;
    private readonly IChatLogParser       ChatLogParser;
    private readonly IChatMessageParser   ChatMessageParser;
    private readonly IChatReplacer        ChatReplacer;
    
    private bool _handleLogs = false;
    
    public ChatEphemeralHandler(IPetServices petServices, IPettableDatabase database)
    {
        ChatDatabaseHandler = new ChatDatabaseHandler(database, petServices);
        
        ChatMessageParser   = new ChatMessageParser(ChatDatabaseHandler, petServices);
        ChatLogParser       = new ChatLogParser(ChatDatabaseHandler, petServices);
        ChatReplacer        = new ChatReplacer(ChatDatabaseHandler, petServices);
    }
    
    public override void OnDispose()
    {
        ChatDatabaseHandler.Dispose();
    }
    
    public override void OnEnable()
    {
        _handleLogs = true;
    }

    public override void OnDisable()
    {
        _handleLogs = false;
    }
    
    public void OnChatClear()
    {
        ChatDatabaseHandler.Clear();
    }
    
    public bool HasChatMessage(uint messageId)
    {
        foreach (IEphemeralChatElement chatElement in ChatDatabaseHandler.ChatElementDatabase.Elements)
        {
            if (chatElement.MessageId != messageId)
            {
                continue;
            }
            
            return true;
        }
        
        return false;
    }
    
    public void OnChatLog(uint messageId, XivChatType xivChatType, uint logMessageId, ILogMessageEntity? sourceEntity, ILogMessageEntity? targetEntity)
    {
        ChatLogParser.OnChatLog(messageId, xivChatType, logMessageId, sourceEntity, targetEntity);
    }
    
    public void OnChatMessage(uint messageId, XivChatType xivChatType)
    {
        ChatMessageParser.OnChatMessage(messageId, xivChatType);
    }
    
    public byte[]? Replace(Utf8String* message, int index)
    {
        if (!_handleLogs)
        {
            return null;
        }
        
        return ChatReplacer.Replace(message, index);
    }
}