using Dalamud.Game.Text;
using PetRenamer.PetNicknames.ChatEphemiral.ChatEntities.Interfaces;
using PetRenamer.PetNicknames.ChatEphemiral.Interfaces;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Enums;
using PetRenamer.PetNicknames.Services.ServiceWrappers.Interfaces;

namespace PetRenamer.PetNicknames.ChatEphemiral.ChatParsers.Interfaces;

internal interface IChatLogPetParserElement : IChatLogParserElement
{  
    NameType       ReplaceNameType { get; }
    IPetSheetData? UsedData        { get; }
    
    bool      IsMyParser(XivChatType chatType);
    IChatPet? Parse(uint messageId, IChatPlayer? chatPlayer); 
}