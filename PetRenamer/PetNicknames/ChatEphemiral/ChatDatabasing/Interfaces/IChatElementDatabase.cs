using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatDatabasing.Interfaces;

internal interface IChatElementDatabase : IChatDatabase<IEphemeralChatElement>
{
    IEphemeralChatElement? GetChatElement(int id);
    void                   AddChatElement(NameType replaceNameType, IPetSheetData? replaceData, uint messageId, XivChatType chatType, IChatPlayer? sourcePlayer, IChatPlayer? targetPlayer, IChatPet? sourcePet, IChatPet? targetPet);
    void                   AddChatElement(IEphemeralChatElement chatElement);
    void                   RemoveElement(IEphemeralChatElement chatElement);
}