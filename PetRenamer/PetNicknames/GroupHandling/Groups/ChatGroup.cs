using PetRenamer.PetNicknames.Chat.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.PettableDatabase.Interfaces;
using PetRenamer.PetNicknames.Services;

namespace PetRenamer.PetNicknames.GroupHandling.Groups;

internal class ChatGroup : HandlerGroup
{
    public ChatGroup(IChatHandler chatHandler, IEphemaralChatHandler ephemeralChatHandler, IDirtyListener dirtyListener) 
        : base("ChatReplaceType", new EnablableRegistration(chatHandler, "StaticChat"), new EnablableRegistration(ephemeralChatHandler, "EphemeralChat"), dirtyListener) 
        { }

    public override ref Configuration.GroupConfig GetGroupConfig(Configuration configuration)
        => ref configuration.ChatModeGroup;
}