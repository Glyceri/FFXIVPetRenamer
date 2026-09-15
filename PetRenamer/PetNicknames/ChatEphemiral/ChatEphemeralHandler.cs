using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.System.String;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatReplacing;
using PetRenamer.PetNicknames.ChatEphemiral.ChatReplacing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.Services;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.ChatEphemiral;

// Why did I decide I wanted THIS :sob:
// But it is soooo cool :sob:
// It like retroactively renames the pets, and if you change name
// that reflects too. And if you close the plugin its gone, like it should have always been.
// It's funny that the retroactive renaming is a restriction I can;t even really NOT do :icant:
internal unsafe class ChatEphemeralHandler : EnablableHandler, IEphemaralChatHandler
{
    private readonly IPetServices       PetServices;
    private readonly IChatLogParser     ChatLogParser;
    private readonly IChatMessageParser ChatMessageParser;
    private readonly IChatReplacer      ChatReplacer;
    
    private bool _handleLogs = false;
    
    public ChatEphemeralHandler(IPetServices petServices)
    {
        PetServices       = petServices;
        ChatMessageParser = new ChatMessageParser(petServices);
        ChatLogParser     = new ChatLogParser(petServices);
        ChatReplacer      = new ChatReplacer(petServices);
    }
    
    public override void OnDispose()
        { }
    
    public override void OnEnable()
    {
        _handleLogs = true;
    }

    public override void OnDisable()
    {
        _handleLogs = false;
    }
    
    public bool HasChatMessage(uint messageId)
    {
        foreach (IEphemeralChatElement chatElement in PetServices.ChatDatabaseService.ChatElementDatabase.Elements)
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