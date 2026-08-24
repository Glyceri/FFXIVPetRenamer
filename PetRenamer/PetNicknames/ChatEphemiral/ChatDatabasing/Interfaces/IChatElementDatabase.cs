using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;

internal interface IChatElementDatabase : IChatDatabase<IEphemeralChatElement>
{
    const uint MAX_CHAT_ELEMENTS = 4096;    // I do not know the vanilla game max chat log count, but this is reasonable enough :shrug:
    const uint CLEANUP_COUNT     = 128;
    
    IEphemeralChatElement? GetChatElement(int id);
    void                   AddChatElement(NameType replaceNameType, IPetSheetData? replaceData, uint messageId, uint logMessageId, XivChatType chatType, IChatPlayer? sourcePlayer, IChatPlayer? targetPlayer, IChatPet? sourcePet, IChatPet? targetPet);
    void                   AddChatElement(IEphemeralChatElement chatElement);
    void                   RemoveElement(IEphemeralChatElement chatElement);
    void                   CleanUp();
}