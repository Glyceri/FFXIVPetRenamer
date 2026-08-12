using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Enums;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.PettableUsers.Interfaces;
using PetRenamer.PetNicknames.Services.Interface;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Pet;

internal class EmoteChatLogParserElement : IChatLogPetParserElement
{
    private readonly IPetServices     PetServices;
    private readonly IChatPetDatabase PetDatabase;
    
    public EmoteChatLogParserElement(IChatPetDatabase petDatabase, IPetServices petServices)
    {
        PetDatabase = petDatabase;
        PetServices = petServices;
    }
    
    public NameType ReplaceNameType
        => NameType.Pronoun;
    
    public IPetSheetData? UsedData
        { get; private set; } = null;
    
    public bool IsMyParser(XivChatType chatType)
    {
        return (chatType == XivChatType.StandardEmote);
    }

    public IChatPet? Parse(uint messageId, IChatPlayer? chatPlayer)
    {
        UsedData = null;
        
        if (chatPlayer == null)
        {
            return null;
        }
        
        if (chatPlayer.Strength != MessageStrength.Strong)
        {
            return null;
        }
        
        IPettableUser? user = PetServices.UserList.GetUserFromContentId(chatPlayer.ContentId);
        
        if (user == null)
        {
            return null;
        }
        
        IPettableEntity? target = PetServices.TargetManager.GetLeadingTarget(user);
        
        if (target is not IPettablePet pet)
        {
            return null;
        }
        
        if (pet.Owner == null)
        {
            return null;
        }
        
        UsedData = pet.PetData;
        
        return PetDatabase.MakeChatPet(pet.SkeletonId, pet.Owner.DataBaseEntry.Name, pet.Owner.DataBaseEntry.Homeworld);
    }
}