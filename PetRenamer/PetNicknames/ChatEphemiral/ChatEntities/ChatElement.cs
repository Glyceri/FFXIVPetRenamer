using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatEntities;

internal class ChatElement : IEphemeralChatElement
{
    public uint         MessageId     { get; private set; }
    public XivChatType  ChatType      { get; private set; }
    public string       ReplaceString { get; private set; } = string.Empty;
    public IChatPlayer? SourcePlayer  { get; private set; }
    public IChatPlayer? TargetPlayer  { get; private set; }
    public IChatPet?    SourcePet     { get; private set; }
    public IChatPet?    TargetPet     { get; private set; }

    public ChatElement(uint messageId, XivChatType chatType, string replaceString, IChatPlayer? sourcePlayer, IChatPlayer? targetPlayer, IChatPet? sourcePet, IChatPet? targetPet)
    {
        MessageId     = messageId;
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