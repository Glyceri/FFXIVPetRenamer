using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.Interfaces;

internal interface IEphemeralChatElement
{
    uint         MessageId     { get; }
    uint         LogMessageId  { get; }
    XivChatType  ChatType      { get; }
    string       ReplaceString { get; }
    IChatPlayer? SourcePlayer  { get; }
    IChatPlayer? TargetPlayer  { get; }
    IChatPet?    SourcePet     { get; }
    IChatPet?    TargetPet     { get; }
    
    void SetReplaceString(string replaceString);
}