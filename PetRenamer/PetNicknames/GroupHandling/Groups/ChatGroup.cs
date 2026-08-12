using PetRenamer.PetNicknames.Chat.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;

namespace PetRenamer.PetNicknames.GroupHandling.Groups;

internal class ChatGroup : HandlerGroup
{
    public ChatGroup(IChatHandler chatHandler, IEphemaralChatHandler ephemeralChatHandler, IDirtyListener dirtyListener) 
        : base(chatHandler, ephemeralChatHandler, dirtyListener) 
        { }

    public override ref Configuration.GroupConfig GetGroupConfig(Configuration configuration)
        => ref configuration.ChatModeGroup;
}