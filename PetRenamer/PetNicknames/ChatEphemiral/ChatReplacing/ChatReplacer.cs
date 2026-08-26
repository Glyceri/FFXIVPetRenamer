using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatReplacing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatReplacing;

internal unsafe class ChatReplacer : IChatReplacer
{
    private readonly IChatDatabaseHandler ChatDatabase;
    private readonly IPetServices         PetServices;
    
    public ChatReplacer(IChatDatabaseHandler chatHandler, IPetServices petServices)
    {
        ChatDatabase = chatHandler;
        PetServices  = petServices;
    }
    
    public byte[]? Replace(Utf8String* message, int index)
    {
        if (message == null)
        {
            return null;
        }
        
        IEphemeralChatElement? chatElement = ChatDatabase.ChatElementDatabase.GetChatElement(index);
        
        if (chatElement == null)
        {
            return null;
        }
        
        IChatPet? pet = chatElement.TargetPet ?? chatElement.SourcePet;
        
        if (pet == null)
        {
            return null;
        }
        
        IPettableUser? user = PetServices.UserList.GetUser(pet.Owner.PlayerName, pet.Owner.Homeworld);
        
        if (user == null)
        {
            return null;
        }
        
        using Utf8String editableString = new Utf8String();
        
        editableString.Copy(message);
        
        SeString editableSeString = SeString.Parse(editableString.AsReadOnlySeString());
        
        // TODO: This colour config stuff works for now, but ofc isnt scalable in case I want finer control later.
        Configuration.ColourConfig colourConfig = PetServices.Configuration.ShowInBattleChatColour;
        
        if (chatElement.ChatType == XivChatType.StandardEmote)
        {
            colourConfig = PetServices.Configuration.ShowOnEmotesColour;
        }
        
        
        if (!PetServices.StringHelper.ReplaceSeString(colourConfig, ref editableSeString, pet.Pet, chatElement.ReplaceString, user))
        {
            return null;
        }
        
        return editableSeString.EncodeWithNullTerminator();
    }
}