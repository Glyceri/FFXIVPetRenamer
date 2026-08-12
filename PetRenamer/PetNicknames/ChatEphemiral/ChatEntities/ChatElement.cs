using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatEntities;

internal class ChatElement : IEphemeralChatElement
{
    public uint         MessageId     { get; }
    public uint         LogMessageId  { get; }
    public XivChatType  ChatType      { get; }
    public string       ReplaceString { get; private set; } = string.Empty;
    public IChatPlayer? SourcePlayer  { get; }
    public IChatPlayer? TargetPlayer  { get; }
    public IChatPet?    SourcePet     { get; }
    public IChatPet?    TargetPet     { get; }

    public ChatElement(uint messageId, XivChatType chatType, uint logMessageId, string replaceString, IChatPlayer? sourcePlayer, IChatPlayer? targetPlayer, IChatPet? sourcePet, IChatPet? targetPet)
    {
        MessageId     = messageId;
        LogMessageId  = logMessageId;
        ChatType      = chatType;
        SourcePlayer  = sourcePlayer;
        TargetPlayer  = targetPlayer;
        SourcePet     = sourcePet;
        TargetPet     = targetPet;
        ReplaceString = replaceString;
    }
        
    public void SetReplaceString(string replaceString)
        => ReplaceString = replaceString;
        
    public override string ToString()
        => $"ChatElement: {MessageId}, {ChatType}, ['{ReplaceString}'], [{SourcePlayer?.PlayerName}@{SourcePlayer?.Homeworld}], [{TargetPlayer?.PlayerName}@{TargetPlayer?.Homeworld}], [{SourcePet?.Pet}: {SourcePet?.Owner.PlayerName}@{SourcePet?.Owner.Homeworld}], [{TargetPet?.Pet}: {TargetPet?.Owner.PlayerName}@{TargetPet?.Owner.Homeworld}].";
}